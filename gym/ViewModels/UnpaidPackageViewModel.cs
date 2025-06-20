namespace gym.ViewModels
{
    public class UnpaidPackageViewModel
    {
        public int PaymentId { get; set; }
        public decimal Total { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Description { get; set; }
        public bool IsPaid { get; set; }
    }
}
