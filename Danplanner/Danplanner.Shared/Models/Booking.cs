using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danplanner.Shared.Models
{
    internal class Booking
    {
        public int BookingId { get; set; }
        public int BookingResidents { get; set; }
        public double BookingPrice { get; set; }
    }
}
