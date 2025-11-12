using System.ComponentModel.DataAnnotations;

namespace Danplanner.Domain.Entities
{
    internal class PaymentConfirmation
    {
        [Key]
        public int PaymentConfirmationId { get; set; }
        [Required]
        public DateTime PaymentConfirmationDate { get; set; }
    }
}
