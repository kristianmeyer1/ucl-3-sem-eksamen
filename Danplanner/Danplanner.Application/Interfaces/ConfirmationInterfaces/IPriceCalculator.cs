using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danplanner.Application.Interfaces.ConfirmationInterfaces
{
    public interface IPriceCalculator
    {
        Task<decimal> CalculateAsync(
            int accommodationId,
            List<int> selectedAddonIds,
            DateTime? checkIn,
            DateTime? checkOut);
    }
}
