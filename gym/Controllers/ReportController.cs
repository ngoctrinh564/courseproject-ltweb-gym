using gym.Data;
using gym.ViewModels;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Globalization;
using static iText.Kernel.Font.PdfFontFactory;


public class ReportController : Controller
{
    private readonly GymContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    public ReportController(GymContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<IActionResult> Revenue(int year = 0, int? month = null, int? quarter = null)
    {
        if (year == 0) year = DateTime.Now.Year;

        var payments = await _context.Payments
            .Where(p => p.IsPaid && p.DueDate.HasValue)
                .Select(p => new {
                    Payment = p,
                    DueDate = p.DueDate.Value
                })
                .Where(p => p.DueDate.Year == year &&
                           (!month.HasValue || p.DueDate.Month == month) &&
                           (!quarter.HasValue || ((p.DueDate.Month - 1) / 3 + 1) == quarter))
                .Select(p => p.Payment)
                .Join(_context.MemberPayments,
                p => p.PaymentId,
                mp => mp.PaymentId,
                (p, mp) => new { p, mp })
            .Join(_context.Members,
                temp => temp.mp.MemberId,
                m => m.MemberId,
                (temp, m) => new RevenueReportViewModel.PaymentDetail
                {
                    PaymentDate = (DateTime)temp.p.DueDate,
                    MemberName = m.FullName,
                    Method = temp.p.Method,
                    Total = temp.p.Total
                })
            .ToListAsync();

        var model = new RevenueReportViewModel
        {
            Year = year,
            Month = month,
            Quarter = quarter,
            TotalRevenue = payments.Sum(x => x.Total),
            PaymentDetails = payments
        };

        return View(model);
    }



    public async Task<IActionResult> ExportToExcel(int year, int? month, int? quarter)
    {
        var model = await GetRevenueData(year, month, quarter);

        // ✅ Đúng cách cấu hình license EPPlus 8
        ExcelPackage.License.SetNonCommercialPersonal("leduyhai090704@gmail.com");

        using (var package = new ExcelPackage())
        {
            var ws = package.Workbook.Worksheets.Add("DoanhThu");

            ws.Cells["A1"].Value = "Ngày thanh toán";
            ws.Cells["B1"].Value = "Thành viên";
            ws.Cells["C1"].Value = "Phương thức";
            ws.Cells["D1"].Value = "Số tiền";

            int row = 2;
            foreach (var item in model.PaymentDetails)
            {
                ws.Cells[row, 1].Value = item.PaymentDate.ToString("dd/MM/yyyy");
                ws.Cells[row, 2].Value = item.MemberName;
                ws.Cells[row, 3].Value = item.Method;
                ws.Cells[row, 4].Value = item.Total;
                row++;
            }

            ws.Cells[$"A{row}"].Value = "Tổng doanh thu";
            ws.Cells[$"D{row}"].Value = model.TotalRevenue;

            var stream = new MemoryStream(package.GetAsByteArray());
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"DoanhThu_{year}.xlsx");
        }
    }

    public async Task<IActionResult> ExportToPdf(int year, int? month, int? quarter)
    {
        var model = await GetRevenueData(year, month, quarter);

        byte[] pdfBytes;

        using (var ms = new MemoryStream())
        {
            var writer = new PdfWriter(ms);
            var pdf = new PdfDocument(writer);
            var doc = new Document(pdf);

            var fontPath = Path.Combine(_webHostEnvironment.WebRootPath, "fonts", "Roboto-Regular.ttf");
            var vietnameseFont = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H, EmbeddingStrategy.PREFER_EMBEDDED);

            doc.SetFont(vietnameseFont); // Đặt font mặc định cho document

            // Tiêu đề
            doc.Add(new Paragraph("Báo cáo doanh thu")
                .SetFontSize(16)
                .SetBold()
                .SetTextAlignment(TextAlignment.CENTER));

            doc.Add(new Paragraph($"Năm: {year} | Tháng: {month?.ToString() ?? "-"} | Quý: {quarter?.ToString() ?? "-"}")
                .SetFontSize(11)
                .SetMarginBottom(10));

            // Bảng dữ liệu
            var table = new Table(UnitValue.CreatePercentArray(new float[] { 2, 3, 3, 2 })).UseAllAvailableWidth();

            // Header
            table.AddHeaderCell(new Paragraph("Ngày thanh toán").SetBold());
            table.AddHeaderCell(new Paragraph("Thành viên").SetBold());
            table.AddHeaderCell(new Paragraph("Phương thức").SetBold());
            table.AddHeaderCell(new Paragraph("Số tiền").SetBold());

            // Nội dung
            foreach (var item in model.PaymentDetails)
            {
                table.AddCell(new Paragraph(item.PaymentDate.ToString("dd/MM/yyyy")));
                table.AddCell(new Paragraph(item.MemberName ?? ""));
                table.AddCell(new Paragraph(item.Method ?? ""));
                table.AddCell(new Paragraph(item.Total.ToString("N0", new CultureInfo("vi-VN"))));
            }

            // Tổng cộng
            table.AddCell(new Cell(1, 3).Add(new Paragraph("Tổng doanh thu").SetBold()));
            table.AddCell(new Paragraph(model.TotalRevenue.ToString("N0", new CultureInfo("vi-VN"))).SetBold());

            doc.Add(table);
            doc.Close();

            //ms.Position = 0;
            pdfBytes = ms.ToArray();
        }

        return File(pdfBytes, "application/pdf", $"DoanhThu_{year}.pdf");
    }


    private async Task<RevenueReportViewModel> GetRevenueData(int year, int? month, int? quarter)
    {
        var payments = await _context.Payments
            .Where(p => p.IsPaid && p.DueDate.HasValue)
            .Select(p => new {
                Payment = p,
                DueDate = p.DueDate.Value
            })
            .Where(p => p.DueDate.Year == year &&
                       (!month.HasValue || p.DueDate.Month == month) &&
                       (!quarter.HasValue || ((p.DueDate.Month - 1) / 3 + 1) == quarter))
            .Select(p => p.Payment)

            .Join(_context.MemberPayments,
                p => p.PaymentId,
                mp => mp.PaymentId,
                (p, mp) => new { p, mp })
            .Join(_context.Members,
                temp => temp.mp.MemberId,
                m => m.MemberId,
                (temp, m) => new RevenueReportViewModel.PaymentDetail
                {
                    PaymentDate = (DateTime)temp.p.DueDate,
                    MemberName = m.FullName,
                    Method = temp.p.Method,
                    Total = temp.p.Total
                })
            .ToListAsync();

        return new RevenueReportViewModel
        {
            Year = year,
            Month = month,
            Quarter = quarter,
            TotalRevenue = payments.Sum(x => x.Total),
            PaymentDetails = payments
        };
    }

}
