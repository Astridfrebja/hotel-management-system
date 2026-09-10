using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SamletInfo.Data;
using SamletInfo.Models;
using SamletInfo.Services;

namespace SamletInfo.Controllers
{
    public class BookingController : Controller
    {
        private readonly HotelContext _context;

        public BookingController(HotelContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Search");
        }

        public IActionResult Search()
        {
            ViewBag.IsLoggedIn = IsLoggedIn();
            return View(new RoomSearchViewModel());
        }

        [HttpPost]
        public IActionResult Search(RoomSearchViewModel searchViewModel)
        {
            if (searchViewModel.CheckInDate == null || searchViewModel.EndDate == null)
            {
                ModelState.AddModelError(string.Empty, "Velg fra- og tildato.");
                searchViewModel.AvailableRooms = new List<Room>();
                ViewBag.IsLoggedIn = IsLoggedIn();
                return View(searchViewModel);
            }

            var from = searchViewModel.CheckInDate.Value.Date;
            var to = searchViewModel.EndDate.Value.Date;
            if (to <= from)
            {
                ModelState.AddModelError(string.Empty, "Utsjekk må være etter innsjekk.");
                searchViewModel.AvailableRooms = new List<Room>();
                ViewBag.IsLoggedIn = IsLoggedIn();
                return View(searchViewModel);
            }

            var rooms = _context.Rooms
                .Where(r =>
                    (searchViewModel.NumberOfBeds == null || r.Beds == searchViewModel.NumberOfBeds) &&
                    (string.IsNullOrEmpty(searchViewModel.RoomQuality) || r.Quality == searchViewModel.RoomQuality))
                .ToList()
                .Where(r => RoomAvailability.IsFree(_context, r.Id, from, to))
                .ToList();

            searchViewModel.AvailableRooms = rooms;
            searchViewModel.CheckInDate = from;
            searchViewModel.EndDate = to;
            ViewBag.HasSearched = true;
            ViewBag.IsLoggedIn = IsLoggedIn();
            return View(searchViewModel);
        }

        public IActionResult Book(int id)
        {
            var login = RequireLogin();
            if (login != null)
            {
                return login;
            }

            var room = _context.Rooms.Find(id);
            if (room == null)
            {
                return NotFound();
            }

            ViewBag.RoomId = id;
            return View(new BookingViewModel
            {
                CheckInDate = DateTime.Today,
                CheckOutDate = DateTime.Today.AddDays(2)
            });
        }

        public IActionResult BookDirectly(int roomId, DateTime? checkIn, DateTime? checkOut)
        {
            var login = RequireLogin();
            if (login != null)
            {
                return login;
            }

            var from = (checkIn ?? DateTime.Today).Date;
            var to = (checkOut ?? from.AddDays(3)).Date;
            if (to <= from || !RoomAvailability.IsFree(_context, roomId, from, to))
            {
                TempData["Error"] = "Ingen rom tilgjengelige";
                return RedirectToAction(nameof(Search));
            }

            var email = HttpContext.Session.GetString("UserEmail")!;

            var booking = new Booking
            {
                RoomId = roomId,
                CheckIn = from,
                CheckOut = to,
                CustomerEmail = email,
                Status = "Reserved"
            };

            _context.Bookings.Add(booking);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Book(BookingViewModel bookingViewModel, int roomId)
        {
            var login = RequireLogin();
            if (login != null)
            {
                return login;
            }

            if (bookingViewModel.CheckOutDate <= bookingViewModel.CheckInDate)
            {
                ModelState.AddModelError("CheckOutDate", "Utsjekk må være etter innsjekk.");
            }
            else if (!RoomAvailability.IsFree(_context, roomId, bookingViewModel.CheckInDate, bookingViewModel.CheckOutDate))
            {
                ModelState.AddModelError(string.Empty, "Ingen rom tilgjengelige");
            }

            if (ModelState.IsValid)
            {
                var room = _context.Rooms.Find(roomId);
                if (room != null)
                {
                    var booking = new Booking
                    {
                        RoomId = roomId,
                        CheckIn = bookingViewModel.CheckInDate.Date,
                        CheckOut = bookingViewModel.CheckOutDate.Date,
                        CustomerEmail = HttpContext.Session.GetString("UserEmail")!,
                        Status = "Reserved"
                    };

                    _context.Bookings.Add(booking);
                    _context.SaveChanges();

                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.RoomId = roomId;
            return View(bookingViewModel);
        }

        public IActionResult CancelBooking(int bookingId)
        {
            var booking = _context.Bookings.Find(bookingId);
            if (booking != null)
            {
                var room = _context.Rooms.Find(booking.RoomId);
                if (room != null)
                {
                    room.IsAvailable = true;
                }

                _context.Bookings.Remove(booking);
                _context.SaveChanges();
            }

            return RedirectToAction("MyBookings");
        }

        public IActionResult MyBookings()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail") ?? User?.Identity?.Name;
            var userBookings = _context.Bookings
                .Include(b => b.Room)
                .Where(b => userEmail == null || b.CustomerEmail == userEmail)
                .ToList();

            if (string.IsNullOrEmpty(userEmail))
            {
                userBookings = new List<Booking>();
            }

            return View(userBookings);
        }

        private bool IsLoggedIn()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail"));
        }

        private IActionResult? RequireLogin()
        {
            if (IsLoggedIn())
            {
                return null;
            }

            var returnUrl = Request.Path + Request.QueryString;
            return RedirectToAction("Login", "Account", new { returnUrl });
        }
    }
}
