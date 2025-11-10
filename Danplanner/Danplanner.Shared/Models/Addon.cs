using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danplanner.Shared.Models
{
    internal class Addon
    {
        public int AddonId { get; set; }
        public string AddonName { get; set; }
        public double AddonPrice { get; set; }
        public string AddonDescription { get; set; }
    }
}
