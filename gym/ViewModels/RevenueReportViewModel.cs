namespace gym.ViewModels
{
    public class RevenueReportViewModel
    {
        public int Year { get; set; }
        public int? Month { get; set; }
        public int? Quarter { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<PaymentDetail> PaymentDetails { get; set; }

        public class PaymentDetail
        {
            public DateTime PaymentDate { get; set; }
            public string MemberName { get; set; }
            public string Method { get; set; }
            public decimal Total { get; set; }
        }
    }

}
