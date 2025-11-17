using System.ComponentModel.DataAnnotations;

namespace Danplanner.Domain.Entities
{
    public class OrderConfirmation
    {
        [Key]
        public int OrderConfirmationId { get; set; }
        [Required]
        public DateTime OrderConfirmationDate { get; set; }
    }
}
