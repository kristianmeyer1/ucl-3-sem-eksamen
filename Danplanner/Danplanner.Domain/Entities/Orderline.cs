using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danplanner.Domain.Entities
{
    internal class Orderline
    {
        [Key]
        public int OrderlineId { get; set; }
    }
}
