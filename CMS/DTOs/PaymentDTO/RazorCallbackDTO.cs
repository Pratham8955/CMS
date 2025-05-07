namespace CMS.DTOs.PaymentDTO
{
    public class RazorCallbackDTO
    {
        public int StudentId { get; set; }
        public int FeeStructureId { get; set; }
        public string PaymentId { get; set; }
        public string OrderId { get; set; }
        public string Signature { get; set; }

    }
}
