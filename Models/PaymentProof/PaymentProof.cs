using System;

namespace MVCPlayWithMe.Models.PaymentProof
{
    public class PaymentProof
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string TransactionCode { get; set; }
        public int TransferAmount { get; set; }
        public string TransferNote { get; set; }
        public string ImageUrl { get; set; }
        public int? VerifiedBy { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public string Status { get; set; }
        public string RejectReason { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public PaymentProof()
        {
            Status = "pending";
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
    }
}
