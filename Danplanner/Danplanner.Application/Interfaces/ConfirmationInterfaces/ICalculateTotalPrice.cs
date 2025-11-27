using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Danplanner.Application.Models.ModelsDto;

namespace Danplanner.Application.Interfaces.ConfirmationInterfaces
{
    public interface ICalculateTotalPrice
    {
        Task<TotalPriceDto> CalculateAsync(
    int accommodationId,
    List<int> selectedAddonIds,
    int bookingResidents,
    DateTime? checkIn,
    DateTime? checkOut);
    }
}
