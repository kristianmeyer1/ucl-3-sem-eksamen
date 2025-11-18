using Danplanner.Application.Interfaces.AddonInterfaces;
using Danplanner.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Danplanner.Application.Services
{
    public class AddonService : IAddonRepository
    {
        private readonly HttpClient _httpClient;

        public AddonService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<AddonDto>> GetAllAddonsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<AddonDto>>("https://localhost:7026/api/addon");
        }
    }
}
