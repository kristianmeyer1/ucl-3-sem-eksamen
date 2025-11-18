using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danplanner.Application.Models.LoginDto
{
    public class LoginDto
    {
        public string? AdminId { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? Code { get; set; }
    }
}
