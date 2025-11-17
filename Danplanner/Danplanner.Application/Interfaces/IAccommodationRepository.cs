using Danplanner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danplanner.Application.Interfaces
{
    public interface IAccommodationRepository
    {
        Task<IReadOnlyList<Accommodation>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
