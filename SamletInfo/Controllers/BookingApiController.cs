using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SamletInfo.Data;
using SamletInfo.Models;
using SamletInfo.Services;

namespace SamletInfo.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    public class BookingApiController : ControllerBase
    {
        private readonly HotelContext _context;

        public BookingApiController(HotelContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetBookings()
        {
            var bookings = _context.Bookings.Include(b => b.Room).ToList();
            return Ok(bookings);
        }

        [HttpPost]
        public async Task<ActionResult<Booking>> AddBooking(Booking booking)
        {
            if (booking.CheckOut <= booking.CheckIn)
            {
                return BadRequest("Utsjekk må være etter innsjekk.");
            }

            if (!RoomAvailability.IsFree(_context, booking.RoomId, booking.CheckIn, booking.CheckOut))
            {
                return Conflict("Rommet er ikke ledig i den perioden.");
            }

            booking.Status = string.IsNullOrWhiteSpace(booking.Status) ? "Reserved" : booking.Status;
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBookings), new { id = booking.Id }, booking);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateBooking(int id, [FromBody] Booking updatedBooking)
        {
            var booking = _context.Bookings.Find(id);
            if (booking == null)
            {
                return NotFound();
            }

            booking.CheckIn = updatedBooking.CheckIn;
            booking.CheckOut = updatedBooking.CheckOut;
            if (!string.IsNullOrWhiteSpace(updatedBooking.Status))
            {
                booking.Status = updatedBooking.Status;
            }

            _context.SaveChanges();
            return Ok(booking);
        }

        [HttpPost("{id}/checkin")]
        public IActionResult CheckIn(int id)
        {
            var booking = _context.Bookings.Find(id);
            if (booking == null)
            {
                return NotFound();
            }

            booking.Status = "CheckedIn";
            var room = _context.Rooms.Find(booking.RoomId);
            if (room != null)
            {
                room.IsAvailable = false;
            }

            _context.SaveChanges();
            return Ok(booking);
        }

        [HttpPost("{id}/checkout")]
        public IActionResult CheckOut(int id)
        {
            var booking = _context.Bookings.Find(id);
            if (booking == null)
            {
                return NotFound();
            }

            booking.Status = "CheckedOut";
            var room = _context.Rooms.Find(booking.RoomId);
            if (room != null)
            {
                room.IsAvailable = false;
            }

            _context.ServiceTasks.Add(new ServiceTask
            {
                RoomId = booking.RoomId,
                Type = "Cleaner",
                Status = "New",
                Note = "Rengjøring etter utsjekk"
            });

            _context.SaveChanges();
            return Ok(booking);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var bookingToDelete = await _context.Bookings.FindAsync(id);
            if (bookingToDelete == null)
            {
                return NotFound();
            }

            int roomId = bookingToDelete.RoomId;
            _context.Bookings.Remove(bookingToDelete);
            await _context.SaveChangesAsync();

            var stillOccupied = _context.Bookings.Any(b =>
                b.RoomId == roomId && (b.Status == "Reserved" || b.Status == "CheckedIn"));
            var roomToUpdate = await _context.Rooms.FindAsync(roomId);
            if (roomToUpdate != null)
            {
                roomToUpdate.IsAvailable = !stillOccupied;
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }
    }
}
