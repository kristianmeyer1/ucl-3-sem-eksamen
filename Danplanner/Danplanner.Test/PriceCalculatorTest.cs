using Danplanner.Application.Interfaces.AddonInterfaces;
using Danplanner.Application.Interfaces.AccommodationInterfaces;
using Danplanner.Application.Models.ModelsDto;
using Danplanner.Domain.Entities;
using Moq;
using Xunit;

namespace Danplanner.Test
{
    [Fact]
    public async Task PriceCalculatorTest()
    {
        // Mock:
        var addonService = new Mock<IAddonGetAll>();
        addonService.Setup(x => x.GetAllAddonsAsync())
            .ReturnsAsync(new List<AddonDto>
            {
                new AddonDto { AddonId = 1, AddonPrice = 100 },
                new AddonDto { AddonId = 2, AddonPrice = 200 }
            });

        // Mock: 
        var accommodationService = new Mock<IAccommodationGetAll>();
        accommodationService.Setup(x => x.GetAllAccommodationsAsync())
            .ReturnsAsync(new List<Accommodation>()
            {
                new Accommodation { AccommodationId = 1, PricePerNight = 100 },
            });

        // Mock:
        converter.Setup(x => x.AccommodationDtoConverter(It.IsAny<IEnumerable<Accommodation>>()))
            .ReturnsAsync(new List<AccommodationDto>()
            {
                new AccommodationDto { AccommodationId = 1, PricePerNight = 100 }
            });

        // 
        var instancedCalculator = new PriceCalculator(
            addonService.Object,
            accommodationService.Object,
            converter.Object);

        // Input til vores test metode
        int accommodationId = 1,
        var selectedAddonIds = new List<int> { 1, 2 };
        DateTime checkIn = new DateTime(2025, 1, 1);
        DateTime checkOut = new DateTime(2025, 1, 4);
        int guests = 2;

        // Kalder metoden
        var result = await instancedCalculator(accommodationId, selectedAddonIds, checkIn, checkOut, guests);

        // Kontrollerer dagberegning

        // Kontrollerer addon beregning

        // Kontrollerer totalpris beregning


    }

    //public class FakeAccommodationGetAll : IAccommodationGetAll
    //{
    //    public Task<IEnumerable<Accommodation>> GetAllAccommodationsAsync()
    //    {
    //        return Task.FromResult<IEnumerable<Accommodation>>(new List<Accommodation>()
    //        {
    //            new Accommodation { AccommodationId = 1, PricePerNight = 100 }
    //        });
    //    }
    //}
}
