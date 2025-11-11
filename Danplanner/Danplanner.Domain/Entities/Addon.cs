using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danplanner.Domain.Entities
{
    internal class Addon
    {
        [Key]
        public int AddonId { get; set; }
        [Required]
        public string AddonName { get; set; }
        [Required]
        public double AddonPrice { get; set; }
        [Required]
        public string AddonDescription { get; set; }
    }
}
