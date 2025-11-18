using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danplanner.Application.Models.LoginDto
{
    public class VerifyCodeDto
    {
        public string UserEmail { get; set; }
        public string Code { get; set; }
    }
}
