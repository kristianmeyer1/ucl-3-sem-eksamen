using System.ComponentModel.DataAnnotations;

namespace Danplanner.Domain.Entities
{
    public class PaymentConfirmation
    {
        [Key]
        public int PaymentConfirmationId { get; set; }
        [Required]
        public DateTime PaymentConfirmationDate { get; set; }
        public int PaymentId { get; set; }
    }
}
