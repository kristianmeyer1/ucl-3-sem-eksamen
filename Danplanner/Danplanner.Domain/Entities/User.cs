using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danplanner.Domain.Entities
{
    internal class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        public string UserAdress { get; set; }
        public int UserMobile { get; set; }
        [Required]
        public string UserEmail { get; set; }
    }
}
