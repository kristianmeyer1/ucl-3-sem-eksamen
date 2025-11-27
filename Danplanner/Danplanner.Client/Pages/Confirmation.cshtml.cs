using Danplanner.Application.Interfaces.AccommodationInterfaces;
using Danplanner.Application.Interfaces.AddonInterfaces;
using Danplanner.Application.Interfaces.BookingInterfaces;
using Danplanner.Application.Interfaces.ConfirmationInterfaces;
using Danplanner.Application.Interfaces.UserInterfaces;
using Danplanner.Application.Models;
using Danplanner.Application.Models.ModelsDto;
using Danplanner.Application.Services;
using Danplanner.Domain.Entities;
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
            // Kontaktinfo boks
            var filePath = Path.Combine(_env.WebRootPath ?? string.Empty, "data", "contactinfo.txt");
            ContactInformation = ContactInfoReader.Load(filePath);

            // Tilkøb
            Addons = (await _addonGetAll.GetAllAddonsAsync()).ToList();

            // Datoer
            DateTime? startDt = _parseDate.ParseDate(Start, out var startDisp);
            DateTime? endDt = _parseDate.ParseDate(End, out var endDisp);
            StartDisplay = startDisp;
            EndDisplay = endDisp;

            if (startDt.HasValue && endDt.HasValue)
            {
                Days = Math.Max(0, (endDt.Value.Date - startDt.Value.Date).Days);
            }

            // Hvis vi ikke fik id via querystring, brug cookie som fallback
            if (!AccommodationId.HasValue)
            {
                var cookieVal = Request.Cookies["selectedItem"];
                if (int.TryParse(cookieVal, out var idFromCookie))
                {
                    AccommodationId = idFromCookie;
                }
            }

            // Hent alle accommodations og find den valgte
            var list = await _accommodationService.GetAccommodationsAsync(startDt, endDt);

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

            // Beregn total (uden tilkøb og personer i første omgang)
            if (SelectedAccommodation?.PricePerNight is decimal price && Days > 0)
            {
                TotalPrice = price * Days;
                TotalPriceDisplay = $"{TotalPrice.Value:N0} kr.";
            }

            // Hvis user er logget ind skal input felter fyles ud
            if (User.Identity?.IsAuthenticated == true)
            {
                int userId = GetLoggedInUserId();
                await UserInputFiller(userId);
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

            var result = await _priceCalculator.CalculateAsync(AccommodationId!.Value, SelectedAddonIds, checkIn, checkOut);

            SelectedAccommodation = result.SelectedAccommodation;
            TotalPrice = result.TotalPrice;
            TotalPriceDisplay = result.TotalPriceDisplay;

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

            // Tjekker om en bruger er logget ind eller ej
            if (User.Identity?.IsAuthenticated == true)
            {
                userId = GetLoggedInUserId();
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    return Page(); // sender brugeren tilbage med fejl
                }

                userId = await NewUserHandler(NewUserEmail, NewUserAdress, NewUserName);
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

        //public async UserHandler(int? userEmail)
        //{

        //}

        public async Task UserInputFiller(int userId)
        {
            UserDto user = await _userGetById.GetUserByIdAsync(userId);
            NewUserName = user.UserName;
            NewUserEmail = user.UserEmail;
            NewUserAdress = user.UserAdress;
        }

        public int GetLoggedInUserId()
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
            if (idClaim != null && int.TryParse(idClaim.Value, out var userId))
            {
                return userId;
            }
            return 0;
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

        
    }
}
