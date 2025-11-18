using Danplanner.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danplanner.Application.Interfaces.AccommodationInterfaces
{
    public interface IAccommodationService
    {
        Task<IReadOnlyList<AccommodationDto>> GetAccommodationsAsync(
            DateTime? start,
            DateTime? end,
            int? daysOverride,
            CancellationToken cancellationToken = default);
    }
}
