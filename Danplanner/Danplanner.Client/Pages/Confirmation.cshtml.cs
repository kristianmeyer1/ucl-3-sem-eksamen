using System.Globalization;
using Danplanner.Application.Interfaces.AccommodationInterfaces;
using Danplanner.Application.Interfaces.AddonInterfaces;
using Danplanner.Application.Interfaces.BookingInterfaces;
using Danplanner.Application.Interfaces.UserInterfaces;
using Danplanner.Application.Models;
using Danplanner.Application.Models.ModelsDto;
using Danplanner.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;

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
        public ContactInformation ContactInformation { get; set; }

        public ConfirmationModel(IAddonGetAll addonGetAll,IAccommodationTransfer accommodationService,IAccommodationUpdate availabilityService, IWebHostEnvironment env, IBookingAdd bookingAdd, IUserAdd userAdd, IUserGetByEmail userGetByEmail)
        {
            _addonGetAll = addonGetAll;
            _accommodationService = accommodationService;
            _availabilityService = availabilityService;
            _env = env;
            _bookingAdd = bookingAdd;
            _userAdd = userAdd;
            _userGetByEmail = userGetByEmail;
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

        [BindProperty]
        public int BookingResidents { get; set; } = 1;

        [BindProperty]
        public int? CurrentUserId { get; set; }

        [BindProperty]
        public List<int> SelectedAddonIds { get; set; } = new();

        [BindProperty]
        public string NewUserName { get; set; }

        [BindProperty]
        public string NewUserEmail { get; set; }

        [BindProperty]
        public string NewUserAdress { get; set; }

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
            DateTime? startDt = ParseDate(Start, out var startDisp);
            DateTime? endDt = ParseDate(End, out var endDisp);
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
            var list = await _accommodationService.GetAccommodationsAsync(startDt, endDt, null);
            if (AccommodationId.HasValue)
            {
                SelectedAccommodation = list
                    .FirstOrDefault(a => a.AccommodationId == AccommodationId.Value);
            }

            // Beregn total (uden tilkøb og personer i første omgang)
            if (SelectedAccommodation?.PricePerNight is decimal price && Days > 0)
            {
                TotalPrice = price * Days;
                TotalPriceDisplay = $"{TotalPrice.Value:N0} kr.";
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Vi skipper fuldstændig over alt med betaling

            DateTime? checkIn = ParseDate(Start, out _);
            DateTime? checkOut = ParseDate(End, out _);

            await CalculateTotalPriceAsync(checkIn, checkOut);

            // Tjekker om vi har det dato info vi skal bruge
            if (!checkIn.HasValue || !checkOut.HasValue || checkIn >= checkOut)
            {
                ModelState.AddModelError("", "Der skete en fejl med datoerne.");
                return Page();
            }

            int userId;

            // Tjekker om en bruger er logget ind eller ej
            if (CurrentUserId.HasValue)
            {
                userId = CurrentUserId.Value;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(NewUserName) ||
                    string.IsNullOrWhiteSpace(NewUserEmail) ||
                    string.IsNullOrWhiteSpace(NewUserAdress))
                {
                    ModelState.AddModelError("", "Venligst udfyld alle informations bokse.");
                    return Page();
                }

                // Vi opretter nu den nye bruger
                var newUser = new UserDto()
                {
                    UserName = NewUserName,
                    UserEmail = NewUserEmail,
                    UserAdress = NewUserAdress,
                };

                // Sender den nye bruger til oprettelse
                await _userAdd.AddUserAsync(newUser);
                var createdUser = await _userGetByEmail.GetUserByEmailAsync(NewUserEmail);
                userId = createdUser.UserId;
            }

            var bookingDto = new BookingDto()
            {
                BookingResidents = BookingResidents,
                BookingPrice = (double)TotalPrice.Value,
                CheckInDate = checkIn.Value,
                CheckOutDate = checkOut.Value,
                UserId = userId, 
                AccommodationId = SelectedAccommodation.AccommodationId
            };

            try
            {
                await _availabilityService.MarkUnavailableAsync(UnitId.Value);
                await _bookingAdd.AddBookingAsync(bookingDto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Der skete en fejl.");
                return Page();
            }

            return RedirectToPage("/ThankYou");
        }

        private static DateTime? ParseDate(string? raw, out string display)
        {
            display = "—";
            if (string.IsNullOrEmpty(raw))
                return null;

            if (DateTime.TryParseExact(
                    raw,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var dt))
            {
                display = dt.ToString("d. MMMM", CultureInfo.GetCultureInfo("da-DK"));
                return dt.Date;
            }

            display = raw;
            return null;
        }

        private async Task CalculateTotalPriceAsync(DateTime? checkIn, DateTime? checkOut)
        {
            // Vi genindlæser addon listen
            Addons = (await _addonGetAll.GetAllAddonsAsync()).ToList();

            // Vi beregner hvor mange dage
            if (checkIn.HasValue && checkOut.HasValue)
                Days = Math.Max(0, (checkOut.Value.Date - checkIn.Value.Date).Days);

            // Vi genindlæser den valgte hytte/plads
            var accommodations = await _accommodationService.GetAccommodationsAsync(checkIn, checkOut, null);
            SelectedAccommodation = accommodations.FirstOrDefault(a => a.AccommodationId == AccommodationId);

            // Standard pris for valgt hytte/plads
            if (SelectedAccommodation?.PricePerNight is decimal price)
                TotalPrice = price * Days;
            else
                TotalPrice = 0;

            // Addon price
            AddonsTotal = Addons
                .Where(a => SelectedAddonIds.Contains(a.AddonId))
                .Sum(a => (decimal)a.AddonPrice);

            TotalPrice += AddonsTotal;

            // Det skal koste ekstra for hver voksen
            if (BookingResidents > 1)
            {
                int extraAdults = BookingResidents - 1;
                TotalPrice += extraAdults * 350;
            }

            TotalPriceDisplay = $"{TotalPrice.Value:N0} kr.";
        }
    }
}
