using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
