using BCrypt.Net;
using gym.Data;
using gym.DTOs;
using gym.Models.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;

namespace gym.Controllers;

public class AccountController : Controller
{
    private readonly GymContext _context;
    private readonly EmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly IMemoryCache _cache;

    public AccountController(GymContext context, EmailService emailService, IConfiguration configuration, IMemoryCache cache)
    {
        _context = context;
        _emailService = emailService;
        _configuration = configuration;
        _cache = cache;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginDto model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _context.Users.Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.UserName == model.UserName);

        if (user == null)
        {
            TempData["Error"] = "Tên đăng nhập không tồn tại.";
            return View(model);
        }

        if (user.IsAtive == false)
        {
            TempData["Warning"] = "Tài khoản của bạn đã bị vô hiệu hóa.";
            return View(model);
        }

        if (!BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
        {
            TempData["Error"] = "Mật khẩu không chính xác.";
            return View(model);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, user.Role.RoleName),
            new Claim("UserId", user.UserId.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity));

        TempData["Success"] = $"Chào mừng {user.UserName} đăng nhập thành công!";

        return user.Role.RoleName == "Admin"
            ? RedirectToAction("Dashboard", "Admin")
            : RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterDto model)
    {
        if (!ModelState.IsValid) return View(model);

        if (model.Password != model.ConfirmPassword)
        {
            ModelState.AddModelError("ConfirmPassword", "Mật khẩu xác nhận không khớp");
            return View(model);
        }

        if (await _context.Users.AnyAsync(u =>
            (u.UserName != null && u.UserName == model.UserName) ||
            (u.Email != null && u.Email == model.Email)))
        {
            ModelState.AddModelError("", "Tên đăng nhập hoặc email đã tồn tại");
            return View(model);
        }

        HttpContext.Session.SetString("TempRegister_FullName", model.FullName);
        HttpContext.Session.SetString("TempRegister_DateOfBirth", model.DateOfBirth.ToString("yyyy-MM-dd"));
        HttpContext.Session.SetString("TempRegister_Sex", model.Sex.ToString());
        HttpContext.Session.SetString("TempRegister_Phone", model.Phone);
        HttpContext.Session.SetString("TempRegister_Address", model.Address);
        HttpContext.Session.SetString("TempRegister_UserName", model.UserName);
        HttpContext.Session.SetString("TempRegister_Password", model.Password);
        HttpContext.Session.SetString("TempRegister_Email", model.Email);

        var otpCode = new Random().Next(100000, 999999).ToString();
        HttpContext.Session.SetString("RegisterOtp", otpCode);
        HttpContext.Session.SetString("RegisterEmail", model.Email);

        string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "OtpRegisterTemplate.html");
        string emailBody = await System.IO.File.ReadAllTextAsync(templatePath);
        emailBody = emailBody.Replace("{{FULLNAME}}", model.FullName)
                             .Replace("{{OTP}}", otpCode);

        await _emailService.SendEmailAsync(model.Email, "Xác nhận tài khoản GYM Club", emailBody);

        return RedirectToAction("VerifyOtp", new { email = model.Email });
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult ForgotPassword() => View();

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email && u.IsAtive == true);
        if (user == null)
        {
            ModelState.AddModelError("", "❌ Email không tồn tại");
            return View(model);
        }

        var otp = new Random().Next(100000, 999999).ToString();

        HttpContext.Session.SetString("OTP", otp);
        HttpContext.Session.SetString("OTP_Email", model.Email);
        HttpContext.Session.SetString("OTP_Expire", DateTime.UtcNow.AddMinutes(5).ToString());

        await SendOtpEmail(model.Email, otp);

        return RedirectToAction("VerifyOtpForgotPassword", new { email = model.Email });
    }

    [HttpGet]
    public IActionResult VerifyOtp(string email)
    {
        return View(new OtpVerificationDto { Email = email });
    }

    [HttpPost]
    public async Task<IActionResult> VerifyOtp(OtpVerificationDto model)
    {
        var sessionOtp = HttpContext.Session.GetString("RegisterOtp");
        var sessionEmail = HttpContext.Session.GetString("RegisterEmail");

        if (model.Email != sessionEmail || model.OtpCode != sessionOtp)
        {
            ModelState.AddModelError("", "Mã OTP không đúng hoặc đã hết hạn.");
            return View(model);
        }

        try
        {
            var member = new Member
            {
                FullName = HttpContext.Session.GetString("TempRegister_FullName"),
                DateOfBirth = DateTime.Parse(HttpContext.Session.GetString("TempRegister_DateOfBirth")),
                Sex = bool.Parse(HttpContext.Session.GetString("TempRegister_Sex")),
                Phone = HttpContext.Session.GetString("TempRegister_Phone"),
                Address = HttpContext.Session.GetString("TempRegister_Address"),
                CreateDate = DateTime.Now
            };

            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            var user = new User
            {
                RoleId = 2,
                UserName = HttpContext.Session.GetString("TempRegister_UserName"),
                Password = BCrypt.Net.BCrypt.HashPassword(HttpContext.Session.GetString("TempRegister_Password")),
                Email = sessionEmail,
                Status = "Hoạt động",
                IsAtive = true,
                ReferenceId = member.MemberId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Clear session
            HttpContext.Session.Remove("RegisterOtp");
            HttpContext.Session.Remove("RegisterEmail");
            HttpContext.Session.Remove("TempRegister_FullName");
            HttpContext.Session.Remove("TempRegister_DateOfBirth");
            HttpContext.Session.Remove("TempRegister_Sex");
            HttpContext.Session.Remove("TempRegister_Phone");
            HttpContext.Session.Remove("TempRegister_Address");
            HttpContext.Session.Remove("TempRegister_UserName");
            HttpContext.Session.Remove("TempRegister_Password");
            HttpContext.Session.Remove("TempRegister_Email");

            TempData["Success"] = "✅ Xác thực OTP thành công. Bạn có thể đăng nhập!";
            return RedirectToAction("Login");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "❌ Có lỗi xảy ra khi lưu thông tin: " + ex.Message);
            return View(model);
        }
    }
    [HttpGet]
    public IActionResult VerifyOtpForgotPassword(string email)
    {
        return View(new OtpVerificationDto { Email = email });
    }

    [HttpPost]
    public IActionResult VerifyOtpForgotPassword(OtpVerificationDto model)
    {
        var storedOtp = HttpContext.Session.GetString("OTP");
        var storedEmail = HttpContext.Session.GetString("OTP_Email");
        var expireTime = DateTime.Parse(HttpContext.Session.GetString("OTP_Expire"));

        if (model.Email != storedEmail || DateTime.UtcNow > expireTime)
        {
            ModelState.AddModelError("", "❌ Mã OTP đã hết hạn hoặc không hợp lệ.");
            return View(model);
        }

        if (model.OtpCode != storedOtp)
        {
            ModelState.AddModelError("", "❌ Mã OTP không đúng.");
            return View(model);
        }

        TempData["EmailVerified"] = model.Email;
        HttpContext.Session.Remove("OTP");
        HttpContext.Session.Remove("OTP_Email");
        HttpContext.Session.Remove("OTP_Expire");

        return RedirectToAction("ResetPassword");
    }
    [HttpGet]
    public IActionResult ResetPassword()
    {
        if (TempData["EmailVerified"] == null) return RedirectToAction("ForgotPassword");

        ViewBag.Email = TempData["EmailVerified"].ToString();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(string email, string newPassword, string confirmPassword)
    {
        if (newPassword != confirmPassword)
        {
            ModelState.AddModelError("", "❌ Mật khẩu xác nhận không khớp.");
            return View();
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.IsAtive == true);
        if (user == null)
        {
            ModelState.AddModelError("", "❌ Người dùng không tồn tại.");
            return View();
        }

        user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
        await _context.SaveChangesAsync();

        TempData["Success"] = "✅ Mật khẩu đã được đặt lại thành công.";
        return RedirectToAction("Login");
    }

    private async Task SendOtpEmail(string toEmail, string otp)
    {
        string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "OtpEmailTemplate.html");
        string body = await System.IO.File.ReadAllTextAsync(templatePath);
        body = body.Replace("{{OTP}}", otp);

        var mail = new MailMessage
        {
            From = new MailAddress("leduyhai090704@gmail.com", "GYM Club"),
            Subject = "Mã OTP xác thực",
            Body = body,
            IsBodyHtml = true
        };
        mail.To.Add(toEmail);

        using var smtp = new SmtpClient("smtp.gmail.com", 587)
        {
            Credentials = new NetworkCredential("leduyhai090704@gmail.com", "wrdxbdbxngalodhk"),
            EnableSsl = true
        };

        await smtp.SendMailAsync(mail);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> EditProfile()
    {
        var userId = int.Parse(User.FindFirst("UserId")?.Value);
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return NotFound();

        var model = new EditProfileDto
        {
            UserName = user.UserName,
            Email = user.Email
        };
        return View(model);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> EditProfile(EditProfileDto model)
    {
        if (!ModelState.IsValid) return View(model);

        var userId = int.Parse(User.FindFirst("UserId")?.Value);
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return NotFound();

        if (await _context.Users.AnyAsync(u =>
            (u.UserName == model.UserName || u.Email == model.Email) && u.UserId != userId))
        {
            ModelState.AddModelError("", "Tên đăng nhập hoặc email đã tồn tại");
            return View(model);
        }

        user.UserName = model.UserName;
        user.Email = model.Email;
        if (!string.IsNullOrEmpty(model.Password))
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
        }

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }
}
