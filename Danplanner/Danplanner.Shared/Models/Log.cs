using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danplanner.Shared.Models
{
    internal class Log
    {
        public int LogId { get; set; }
        public string LogDescription { get; set; }
        public DateTime LogTimeStamp { get; set; }
    }
}
