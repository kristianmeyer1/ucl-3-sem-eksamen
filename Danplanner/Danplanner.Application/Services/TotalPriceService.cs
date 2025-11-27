using Danplanner.Application.Interfaces.AccommodationInterfaces;
using Danplanner.Application.Interfaces.AddonInterfaces;
using Danplanner.Application.Models.ModelsDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danplanner.Application.Services
{
    public class TotalPriceService
    {
        private readonly IAddonGetAll _addonGetAll;
        private readonly IAccommodationGetAll _accommodationGetAll;
        private readonly IAccommodationConverter _accommodationConverter;

        public TotalPriceService
        (
            IAddonGetAll addonGetAll,
            IAccommodationGetAll accommodationGetAll,
            IAccommodationConverter accommodationConverter)
        {
            _addonGetAll = addonGetAll;
            _accommodationGetAll = accommodationGetAll;
            _accommodationConverter = accommodationConverter;
        }

        public async Task<decimal> CalculateAsync
        (
            int accommodationId,
            List<int> selectedAddonIds,
            DateTime? checkIn,
            DateTime? checkOut)
        {
            var addons = (await _addonGetAll.GetAllAddonsAsync()).ToList();

            int days = 0;
            if (checkIn.HasValue && checkOut.HasValue)
            {
                days = Math.Max(0, (checkOut.Value.Date - checkIn.Value.Date).Days);
            }

            var accommodations = await _accommodationGetAll.GetAllAccommodationsAsync();
            var accommodationsDto = await _accommodationConverter.AccommodationDtoConverter(accommodations);
            var selectedAccommodation = accommodationsDto.FirstOrDefault(a => a.AccommodationId == accommodationId);

            decimal total = 0;
            if (selectedAccommodation?.PricePerNight is decimal price)
            {
                total = price * days;
            }

            var addonsTotal = addons
                .Where(a => selectedAddonIds.Contains(a.AddonId))
                .Sum(a => (decimal)a.AddonPrice);

            total += addonsTotal;

            return total;
        }
    }
}
