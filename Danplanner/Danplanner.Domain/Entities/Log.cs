using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danplanner.Domain.Entities
{
    internal class Log
    {
        [Key]
        public int LogId { get; set; }
        [Required]
        public string LogDescription { get; set; }
        [Required]
        public DateTime LogTimeStamp { get; set; }
    }
}
