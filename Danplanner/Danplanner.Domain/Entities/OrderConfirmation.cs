using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danplanner.Domain.Entities
{
    internal class OrderConfirmation
    {
        [Key]
        public int OrderConfirmationId { get; set; }
        [Required]
        public DateTime OrderConfirmationDate { get; set; }
    }
}
