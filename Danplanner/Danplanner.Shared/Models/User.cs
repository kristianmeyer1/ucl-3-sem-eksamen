using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danplanner.Shared.Models
{
    internal class User
    {
        public int UserId { get; set; }
        public string UserAdress { get; set; }
        public int UserMobile { get; set; }
        public string UserEmail { get; set; }
    }
}
