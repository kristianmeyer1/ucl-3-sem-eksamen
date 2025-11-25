using Danplanner.Application.Interfaces.BookingInterfaces;
using Danplanner.Application.Models.ModelsDto;
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

        public BookingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task AddBookingAsync(BookingDto bookingDto)
        {
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7026/api/booking", bookingDto);
            response.EnsureSuccessStatusCode();
        }
    }
}
