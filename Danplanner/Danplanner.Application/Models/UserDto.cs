using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danplanner.Application.Models
{
    public class UserDto
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        public string UserAdress { get; set; }
        public string UserMobile { get; set; }
        [Required]
        public string UserEmail { get; set; }
        public string? UserName { get; set; }
    }
}
