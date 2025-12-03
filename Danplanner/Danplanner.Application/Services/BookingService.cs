using Danplanner.Application.Interfaces.AccommodationInterfaces;
using Danplanner.Application.Interfaces.BookingInterfaces;
using Danplanner.Application.Interfaces.OrderlineInterfaces;
using Danplanner.Application.Interfaces.UserInterfaces;
using Danplanner.Application.Interfaces.AddonInterfaces;
using Danplanner.Application.Models.ModelsDto;
using Danplanner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Danplanner.Application.Services
{
    public class BookingService : IBookingAdd
    {
        private readonly HttpClient _httpClient;
        private readonly IUserGetById _userGetById;
        private readonly IOrderlineAdd _orderlineAdd;
        private readonly IOrderlineGetById _orderlineGetById;
        private readonly IBookingGetById _bookingGetById;
        private readonly IAccommodationGetById2 _accommodationGetById2;
        private readonly IAddonGetByBookingId _addonGetByBookingId;

        public BookingService
        (
            HttpClient httpClient,
            IUserGetById userGetById,
            IOrderlineAdd orderlineAdd,
            IOrderlineGetById orderlineGetById,
            IBookingGetById bookingGetById,
            IAccommodationGetById2 accommodationGetById2,
            IAddonGetByBookingId addonGetByBookingId
        )
        {
            _httpClient = httpClient;
            _userGetById = userGetById;
            _orderlineAdd = orderlineAdd;
            _orderlineGetById = orderlineGetById;
            _bookingGetById = bookingGetById;
            _accommodationGetById2 = accommodationGetById2;
            _addonGetByBookingId = addonGetByBookingId;
        }

        public async Task AddBookingAsync(BookingDto bookingDto)
        {
            // Send booking til DB
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7026/api/booking", bookingDto);

            // Opret orderline og hent id til faktura
            int orderlineId = await _orderlineAdd.OrderlineAddAsync(bookingDto.BookingId);

            // Hent orderline
            OrderlineDto orderline = await _orderlineGetById.OrderlineGetByIdAsync(orderlineId);

            // Hent booking fra fremmed nøgle
            BookingDto booking = await _bookingGetById.BookingGetByIdAsync(orderline.BookingId);

            // Henter info på vores user
            UserDto user = await _userGetById.GetUserByIdAsync(booking.UserId);

            // Hent accommodation
            AccommodationDto accommodation = await _accommodationGetById2.AccommodationGetByIdAsync(booking.AccommodationId);

            // Hent tilkøb
            //List<AddonDto> addons = await _addonGetByBookingId.GetAddonsByBookingIdAsync(bookingDto.BookingId);

            // Opret bekræftelses objekt og send det til notifikations service
            var orderConfirmation = new OrderConfirmationDto
            {
                Date = DateTime.Now,
                UserEmail = user.UserEmail,
                UserName = user.UserName,
                CheckInDate = bookingDto.CheckInDate,
                CheckOutDate = bookingDto.CheckOutDate,
            };

            await _httpClient.PostAsJsonAsync("http://localhost:8080/orderNotify", orderConfirmation);

            // Opretter faktura objekt og sender det
            var paymentConfirmation = new PaymentConfirmationDto
            {
                UserEmail = user.UserEmail,
                UserName = user.UserName,
                Date = DateTime.Now,

                PaymentId = 0, // Vi har ikke payment etc, så vi sætter id til 0
                OrderlineId = orderlineId,

                AccommodationName = accommodation.AccommodationName,
                BookingResidents = booking.BookingResidents,
                BookingCheckIn = booking.CheckInDate,
                BookingCheckOut = booking.CheckOutDate,
                Price = orderline.TotalPrice,
            };

            await _httpClient.PostAsJsonAsync("http://localhost:8080/paymentNotify", paymentConfirmation);

            response.EnsureSuccessStatusCode();
        }
    }
}
