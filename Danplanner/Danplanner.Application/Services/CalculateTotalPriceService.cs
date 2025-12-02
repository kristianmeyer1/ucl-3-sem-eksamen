using Danplanner.Application.Interfaces.AccommodationInterfaces;
using Danplanner.Application.Interfaces.AddonInterfaces;
using Danplanner.Application.Models.ModelsDto;
using Danplanner.Application.Interfaces.ConfirmationInterfaces;

namespace Danplanner.Application.Services
{
    public class CalculateTotalPriceService : ICalculateTotalPrice
    {
        private readonly IAddonGetAll _addonGetAll;
        private readonly IAccommodationGetAll _accommodationGetAll;
        private readonly IAccommodationConverter _accommodationConverter;

        public CalculateTotalPriceService
        (
            IAddonGetAll addonGetAll,
            IAccommodationGetAll accommodationGetAll,
            IAccommodationConverter accommodationConverter)
        {
            _addonGetAll = addonGetAll;
            _accommodationGetAll = accommodationGetAll;
            _accommodationConverter = accommodationConverter;
        }

        public async Task<TotalPriceDto> CalculateAsync
        (
            int accommodationId,
            List<int> selectedAddonIds,
            DateTime? checkIn,
            DateTime? checkOut,
            int numberofGuests)
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
                total = price * days + (numberofGuests * 50);
            }

            var addonsTotal = addons
                .Where(a => selectedAddonIds.Contains(a.AddonId))
                .Sum(a => (decimal)a.AddonPrice);

            total += addonsTotal;

            return new TotalPriceDto
            {
                Days = days,
                Addons = addons,
                AddonsTotal = addonsTotal,
                TotalPrice = total,
                SelectedAccommodation = selectedAccommodation,
                TotalPriceDisplay = $"{total:N0} kr."
            };
        }
    }
}
