using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danplanner.Domain.Entities
{
    internal class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        [Required]
        public bool PaymentStatus { get; set; }
    }
}
