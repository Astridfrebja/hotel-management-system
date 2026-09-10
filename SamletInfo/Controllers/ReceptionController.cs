using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SamletInfo.Data;
using SamletInfo.Models;
using SamletInfo.Services;

namespace SamletInfo.Controllers
{
    public class ReceptionController : Controller
    {
        private readonly HotelContext _context;

        public ReceptionController(HotelContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("Reception")]
        [Route("Reception/Index")]
        public IActionResult Index()
        {
            ViewBag.Bookings = _context.Bookings.Include(b => b.Room).OrderByDescending(b => b.Id).ToList();
            ViewBag.Rooms = _context.Rooms.OrderBy(r => r.Id).ToList();
            ViewBag.Templates = _context.TaskTemplates.OrderBy(t => t.Type).ThenBy(t => t.Note).ToList();
            ViewBag.Tasks = _context.ServiceTasks.OrderByDescending(t => t.Id).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddBooking(string customerEmail, int roomId, DateTime checkIn, DateTime checkOut)
        {
            if (string.IsNullOrWhiteSpace(customerEmail) || checkOut <= checkIn)
            {
                TempData["Error"] = "Fyll inn e-post og gyldige datoer.";
                return RedirectToAction(nameof(Index));
            }

            if (!RoomAvailability.IsFree(_context, roomId, checkIn, checkOut))
            {
                TempData["Error"] = "Rommet er allerede booket i den perioden.";
                return RedirectToAction(nameof(Index));
            }

            _context.Bookings.Add(new Booking
            {
                CustomerEmail = customerEmail.Trim(),
                RoomId = roomId,
                CheckIn = checkIn.Date,
                CheckOut = checkOut.Date,
                Status = "Reserved"
            });
            _context.SaveChanges();
            TempData["Message"] = "Reservasjon lagret.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteBooking(int bookingId)
        {
            var booking = _context.Bookings.Find(bookingId);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                _context.SaveChanges();
                TempData["Message"] = "Reservasjon slettet.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddTask(int roomId, string type, int templateId)
        {
            var mappedType = type == "Cleaning" ? "Cleaner" : type;
            var template = _context.TaskTemplates.Find(templateId);
            if (template == null || !_context.Rooms.Any(r => r.Id == roomId))
            {
                TempData["Error"] = "Velg rom, type og oppgave.";
                return RedirectToAction(nameof(Index));
            }

            _context.ServiceTasks.Add(new ServiceTask
            {
                RoomId = roomId,
                Type = mappedType,
                Note = template.Note,
                Status = "New"
            });
            _context.SaveChanges();
            TempData["Message"] = $"Oppgave lagt til for rom {roomId}.";
            return RedirectToAction(nameof(Index));
        }
    }
}
