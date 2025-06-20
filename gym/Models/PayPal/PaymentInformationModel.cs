namespace gym.Models.PayPal
{
    public class PaymentInformationModel
    {
        public int PaymentId { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; } // USD
    }
}
