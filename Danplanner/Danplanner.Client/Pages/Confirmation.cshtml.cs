using Danplanner.Application.Interfaces.AccommodationInterfaces;
using Danplanner.Application.Interfaces.AddonInterfaces;
using Danplanner.Application.Interfaces.BookingInterfaces;
using Danplanner.Application.Interfaces.ConfirmationInterfaces;
using Danplanner.Application.Interfaces.UserInterfaces;
using Danplanner.Application.Models;
using Danplanner.Application.Models.ModelsDto;
using Danplanner.Application.Services;
using Danplanner.Domain.Entities;
using Danplanner.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Danplanner.Client.Pages
{
    public class ConfirmationModel : PageModel
    {
        private readonly IAddonGetAll _addonGetAll;
        private readonly IAccommodationTransfer _accommodationService;
        private readonly IAccommodationUpdate _availabilityService;
        private readonly IWebHostEnvironment _env;
        private readonly IBookingAdd _bookingAdd;
        private readonly IUserAdd _userAdd;
        private readonly IUserGetByEmail _userGetByEmail;
        private readonly IUserGetById _userGetById;
        private readonly IAccommodationGetAll _accommodationGetAll;
        private readonly IAccommodationConverter _accommodationConverter;
        private readonly IAddressService _addressService;
        private readonly ICalculateTotalPrice _priceCalculator;
        private readonly IParseDate _parseDate;
        public ContactInformation ContactInformation { get; set; }

        public ConfirmationModel
        (
            IAddonGetAll addonGetAll,
            IAccommodationTransfer accommodationService,
            IAccommodationUpdate availabilityService, 
            IWebHostEnvironment env, 
            IBookingAdd bookingAdd, 
            IUserAdd userAdd, 
            IUserGetByEmail userGetByEmail, 
            IUserGetById userGetById,
            IAccommodationGetAll accommodationGetAll, 
            IAccommodationConverter accommodationConverter,
            IAddressService addressService,
            ICalculateTotalPrice calculateTotalPrice, 
            IParseDate parseDate)
        {
            _addonGetAll = addonGetAll;
            _accommodationService = accommodationService;
            _availabilityService = availabilityService;
            _env = env;
            _bookingAdd = bookingAdd;
            _userAdd = userAdd;
            _userGetByEmail = userGetByEmail;
            _userGetById = userGetById;
            _accommodationGetAll = accommodationGetAll;
            _accommodationConverter = accommodationConverter;
            _addressService = addressService;
            _priceCalculator = calculateTotalPrice;
            _parseDate = parseDate;
        }

        [BindProperty(SupportsGet = true)]
        public int? UnitId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? PlaceKey { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? AccommodationId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Start { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? End { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? Category { get; set; }

        [BindProperty]
        public int BookingResidents { get; set; } = 1;

        [BindProperty]
        public int? CurrentUserId { get; set; }

        [BindProperty]
        public List<int> SelectedAddonIds { get; set; } = new();

        [BindProperty]
        [Required(ErrorMessage = "Navn skal udfyldes")]
        public string NewUserName { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Email skal udfyldes")]
        [EmailAddress(ErrorMessage = "Indtast en gyldig email")]
        public string NewUserEmail { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Adresse skal udfyldes")]
        public string NewUserAdress { get; set; } = string.Empty;
        [BindProperty]
        public bool AddressConfirmed { get; set; }

        public decimal AddonsTotal { get; set; }

        // ---- til view ----
        public AccommodationDto? SelectedAccommodation { get; private set; }
        public List<AddonDto> Addons { get; private set; } = new();
        public string StartDisplay { get; private set; } = "—";
        public string EndDisplay { get; private set; } = "—";
        public int Days { get; private set; }
        public decimal? TotalPrice { get; private set; }
        public string TotalPriceDisplay { get; private set; } = "—";

        public async Task OnGetAsync()
        {
            // Datoer
            DateTime? startDt = _parseDate.ParseDate(Start, out var startDisp);
            DateTime? endDt = _parseDate.ParseDate(End, out var endDisp);
            StartDisplay = startDisp;
            EndDisplay = endDisp;

            await LoadPageDataAsync(startDt, endDt);

            // Hvis vi ikke fik id via querystring, brug cookie som fallback
            if (!AccommodationId.HasValue)
            {
                var cookieVal = Request.Cookies["selectedItem"];
                if (int.TryParse(cookieVal, out var idFromCookie))
                {
                    AccommodationId = idFromCookie;
                }
            }

            // Hvis user er logget ind skal input felter fyles ud
            if (User.Identity?.IsAuthenticated == true)
            {
                var (userId, adminId, role) = GetLoggedInUser();

                if (role == "User" && userId.HasValue)
                {
                    await UserInputFiller(userId.Value); // fill form for normal user
                }
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Vi skipper fuldstændig over alt med betaling

            DateTime? checkIn = _parseDate.ParseDate(Start, out var startDisp);
            DateTime? checkOut = _parseDate.ParseDate(End, out var endDisp);
            StartDisplay = startDisp;
            EndDisplay = endDisp;
            int userId;

            await LoadPageDataAsync(checkIn, checkOut);

            var result = await _priceCalculator.CalculateAsync(AccommodationId!.Value, SelectedAddonIds, checkIn, checkOut);

            TotalPrice = result.TotalPrice;
            TotalPriceDisplay = result.TotalPriceDisplay;

            // Accommodation
            var list = await _accommodationGetAll.GetAllAccommodationsAsync();
            var listDto = await _accommodationConverter.AccommodationDtoConverter(list);
            if (AccommodationId.HasValue)
            {
                SelectedAccommodation = listDto.FirstOrDefault(a => a.AccommodationId == AccommodationId.Value);
            }

            // Tjekker om vi har det dato info vi skal bruge
            if (!checkIn.HasValue || !checkOut.HasValue || checkIn >= checkOut)
            {
                ModelState.AddModelError("", "Der skete en fejl med datoerne.");
                return Page();
            }

            if (SelectedAccommodation == null || !AccommodationId.HasValue)
            {
                ModelState.AddModelError("", "Der skete en fejl med den valgte enhed.");
                return Page();
            }

            // Tjekker om en bruger er logget ind eller ej;
            if (User.Identity?.IsAuthenticated == true)
            {
                var (loggedUserId, adminId, role) = GetLoggedInUser();

                if (role == "User" && loggedUserId.HasValue)
                {
                    userId = loggedUserId.Value;
                }
                else
                {
                    ModelState.AddModelError("", "Admins cannot create bookings.");
                    return Page();
                }
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    return Page(); // sender brugeren tilbage med fejl
                }
                
                // Før vi opretter ny bruger, tjekker vi om den findes i forvejen
                var userExists = await _userGetByEmail.GetUserByEmailAsync(NewUserEmail);
                if (userExists == null)
                {
                    userId = await NewUserHandler(NewUserEmail, NewUserAdress, NewUserName);
                }
                else
                {
                    ModelState.AddModelError("", "En bruger med denne mail adresse findes allerede.");
                    return Page();
                }
            }
            if (User.Identity?.IsAuthenticated != true)
            {
                if (!AddressConfirmed)
                {
                    ModelState.AddModelError(nameof(NewUserAdress), "Vælg en adresse fra listen.");
                }
                if (!ModelState.IsValid)
                {
                    return Page(); // sender brugeren tilbage med fejl
                }
            }

            var bookingDto = new BookingDto()
            {
                BookingResidents = BookingResidents,
                BookingPrice = (double)TotalPrice.Value,
                CheckInDate = checkIn.Value,
                CheckOutDate = checkOut.Value,
                UserId = userId, 
                AccommodationId = AccommodationId.Value
            };

            try
            {
                await _bookingAdd.AddBookingAsync(bookingDto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Der skete en fejl.");
                return Page();
            }

            return RedirectToPage("/ThankYou");
        }

        public async Task UserInputFiller(int userId)
        {
            UserDto user = await _userGetById.GetUserByIdAsync(userId);
            NewUserName = user.UserName;
            NewUserEmail = user.UserEmail;
            NewUserAdress = user.UserAdress;
        }

        public (int? UserId, int? AdminId, string? Role) GetLoggedInUser()
        {
            if (User?.Identity?.IsAuthenticated != true)
                return (null, null, null);

            var idClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
            var roleClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role);

            if (idClaim == null)
                return (null, null, null);

            if (roleClaim?.Value == "Admin" && int.TryParse(idClaim.Value, out var adminId))
                return (null, adminId, "Admin");

            if ((roleClaim == null || roleClaim.Value != "Admin") && int.TryParse(idClaim.Value, out var userId))
                return (userId, null, "User");

            return (null, null, null);
        }

        public async Task<int> NewUserHandler(string userEmail, string userAdress, string userName)
        {
            var newUser = new UserDto()
            {
                UserName = NewUserName,
                UserEmail = NewUserEmail,
                UserAdress = NewUserAdress,
            };

            // Sender den nye bruger til oprettelse
            await _userAdd.AddUserAsync(newUser);
            var createdUser = await _userGetByEmail.GetUserByEmailAsync(NewUserEmail);
            return createdUser.UserId;
        }

        private async Task LoadPageDataAsync(DateTime? startDt, DateTime? endDt)
        {
            // Kontaktinfo boks
            var filePath = Path.Combine(_env.WebRootPath ?? string.Empty, "data", "contactinfo.txt");
            ContactInformation = ContactInfoReader.Load(filePath);

            // Tilkøb
            Addons = (await _addonGetAll.GetAllAddonsAsync()).ToList();

            // Days
            if (startDt.HasValue && endDt.HasValue)
                Days = Math.Max(0, (endDt.Value.Date - startDt.Value.Date).Days);

            // Hent alle accommodations og find den valgte
            var list = await _accommodationService.GetAccommodationsAsync(startDt, endDt);
            if (AccommodationId.HasValue)
            {
                SelectedAccommodation = list.FirstOrDefault(a => a.AccommodationId == AccommodationId.Value);
            }

            // Henter valgte accommodation 
            if (!string.IsNullOrWhiteSpace(Category))
            {
                SelectedAccommodation = list
                    .FirstOrDefault(a =>
                        string.Equals(a.Category, Category, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                // fallback: første element, hvis der ingen kategori er
                SelectedAccommodation = list.FirstOrDefault();
            }

            // Beregn Pris (uden tilkøb)
            var result = await _priceCalculator.CalculateAsync(AccommodationId!.Value, SelectedAddonIds, startDt, endDt);

            TotalPrice = result.TotalPrice;
            TotalPriceDisplay = result.TotalPriceDisplay;

        }

        public async Task<JsonResult> OnGetAddressSuggestionsAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new JsonResult(new List<string>());
            }
            var addresses = await _addressService.GetAddressesAsync(query);
            return new JsonResult(addresses);
        }
    }
}
