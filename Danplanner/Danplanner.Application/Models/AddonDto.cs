using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danplanner.Application.Models
{
    public class AddonDto
    {
        public int AddonId { get; set; }
        public string AddonName { get; set; }
        public string AddonDescription { get; set; }
        public double AddonPrice { get; set; }
    }
}
