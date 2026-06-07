using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using SamletInfo.Data;
using SamletInfo.Models;
using Microsoft.AspNetCore.Http; 

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
            return RedirectToAction("Search"); // eller return View(); hvis du har en Index.cshtml
        }

        public IActionResult Search()
        {
            return View(new RoomSearchViewModel()); // Pass an instance of the view model
        }


        [HttpPost]
        public IActionResult Search(RoomSearchViewModel searchViewModel)
        {
            var rooms = _context.Rooms
    .Where(r =>
        (searchViewModel.NumberOfBeds == null || r.Beds == searchViewModel.NumberOfBeds) &&
        (string.IsNullOrEmpty(searchViewModel.RoomQuality) || r.Quality == searchViewModel.RoomQuality) &&
        r.IsAvailable) 
    .ToList();


            Console.WriteLine($"Fant {rooms.Count} rom som matcher søket");

            searchViewModel.AvailableRooms = rooms;
            return View(searchViewModel);
        }


        public IActionResult Book(int id)
        {
            var room = _context.Rooms.Find(id);
            if (room == null || !room.IsAvailable) return NotFound();

            // Consider passing relevant room info to the booking view model if needed
            var bookingViewModel = new BookingViewModel();
            // You might want to pre-populate some properties here if necessary
            ViewBag.RoomId = id; // Or pass it within the view model
            return View(bookingViewModel);
        }
        public IActionResult BookDirectly(int roomId)
        {
            var room = _context.Rooms.Find(roomId);
            if (room == null || !room.IsAvailable) return NotFound();

            var booking = new Booking
            {
                RoomId = roomId,
                CheckIn = DateTime.Now,
                CheckOut = DateTime.Now.AddDays(3),
                CustomerEmail = HttpContext.Session.GetString("UserEmail") ?? "anonymous@example.com"
            };

            room.IsAvailable = false;
            _context.Rooms.Update(room); // Oppdaterer rommet i databasen!
            _context.Bookings.Add(booking);
            _context.SaveChanges();

            return RedirectToAction("MyBookings", new { email = booking.CustomerEmail });
        }
 //       [HttpGet]
    //    public IActionResult GetBookings()
  //      {
  //          var bookings = _context.Bookings
   //             .Include(b => b.room) // Henter romdata automatisk!
   //             .ToList();

    //        return Ok(bookings);
  //      }



        [HttpPost]
        public IActionResult Book(BookingViewModel bookingViewModel, int roomId) // Receive the view model and roomId
        {
            if (ModelState.IsValid)
            {
                var room = _context.Rooms.Find(roomId);
                if (room != null)
                {
                    var booking = new Booking
                    {
                        RoomId = roomId,
                        CheckIn = bookingViewModel.CheckInDate,
                        // You'll need to get the CheckOut date from somewhere (another input?)
                        CustomerEmail = User?.Identity?.Name ?? "anonymous@example.com", // Simple way to get a user identifier
                        Room = room
                    };
                    room.IsAvailable = false;

                    _context.Bookings.Add(booking);
                    _context.SaveChanges();

                    return RedirectToAction("MyBookings", new { email = booking.CustomerEmail });
                }
            }
            // If ModelState is not valid or room not found, return the view with the view model
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
                    room.IsAvailable = true; // Gjør rommet ledig igjen
                    _context.Rooms.Update(room); // 🟢 Oppdaterer romstatusen!
                }

                _context.Bookings.Remove(booking);
                _context.SaveChanges();
            }

            return RedirectToAction("MyBookings");
        }


        public IActionResult MyBookings()
        {
            _context.ChangeTracker.Clear(); // 🟢 Sørger for at vi henter ferske data fra databasen!

            var userEmail = User.Identity.Name ?? HttpContext.Session.GetString("UserEmail");
            var userBookings = _context.Bookings
                .Include(b => b.Room)
                .Where(b => b.CustomerEmail == userEmail)
                .ToList();

            return View(userBookings);
        }

    }
}
