using gym.Data;
using gym.Models;
using gym.Models.PayPal;
using gym.Services.PayPal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace gym.Controllers
{
    public class PayPalController : Controller
    {
        private readonly PayPalService _payPalService;
        private readonly GymContext _context;
        private readonly EmailService _emailService;
        public PayPalController(PayPalService payPalService, GymContext context, EmailService emailService)
        {
            _payPalService = payPalService;
            _context = context;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(int paymentId)
        {
            // ✅ Truy payment từ DB
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment == null || payment.IsPaid)
            {
                return NotFound("Không tìm thấy hoặc giao dịch đã thanh toán.");
            }

            // ✅ Chuyển đổi từ VND → USD (ví dụ: chia 25,000)
            double usdAmount = (double)Math.Round(payment.Total / 25000, 2);

            var model = new PaymentInformationModel
            {
                PaymentId = payment.PaymentId,
                Description = $"Thanh toán đơn hàng #{payment.PaymentId}",
                Amount = usdAmount
            };

            var returnUrl = $"{Request.Scheme}://{Request.Host}/PayPal/Success?paymentId={payment.PaymentId}";
            var cancelUrl = $"{Request.Scheme}://{Request.Host}/PayPal/Cancel";

            var url = await _payPalService.CreatePaymentUrl(model, returnUrl, cancelUrl);
            return Redirect(url);
        }


        public async Task<IActionResult> Success(int paymentId)
        {
            // Ví dụ cập nhật lại DB
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment != null && !payment.IsPaid)
            {
                payment.IsPaid = true;
                payment.Method = "PayPal";
                payment.Note = "Thanh toán thành công qua PayPal";
                await _context.SaveChangesAsync();
            }
            // Truy từ Payment → MemberPayment
            var _memberPayment = await _context.MemberPayments
                .Include(mp => mp.Member)
                .FirstOrDefaultAsync(mp => mp.PaymentId == payment.PaymentId);

            if (_memberPayment != null)
            {
                var memberId = _memberPayment.MemberId;

                // Truy tiếp từ Member → User
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.RoleId == 2 && u.ReferenceId == memberId);

                if (user != null && !string.IsNullOrEmpty(user.Email))
                {
                    var member = await _context.Members.FindAsync(memberId);

                    string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "PaymentSuccessTemplate.html");
                    string emailBody = await System.IO.File.ReadAllTextAsync(templatePath);

                    emailBody = emailBody
                        .Replace("{{UserName}}", member.FullName)
                        .Replace("{{PaymentId}}", payment.PaymentId.ToString())
                        .Replace("{{Amount}}", payment.Total.ToString("N0"))
                        .Replace("{{Date}}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));

                    await _emailService.SendEmailAsync(user.Email, "Thanh toán thành công qua VNPay", emailBody);
                }
            }
            ViewBag.PaymentId = paymentId;
            return View("Success");
        }


        public IActionResult Cancel()
        {
            return View("Cancel");
        }
    }
}
