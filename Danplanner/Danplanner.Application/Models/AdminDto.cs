using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danplanner.Application.Models
{
    public class AdminDto
    {
        [Key]
        public int AdminId { get; set; }
        public string? AdminDtoPassword { get; set; }
    }
}
