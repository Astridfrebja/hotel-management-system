using Microsoft.AspNetCore.Mvc;
using SamletInfo.Data;
using SamletInfo.Models;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/bookings")]
public class BookingApiController : ControllerBase
{
    private readonly HotelContext _context;
    private readonly ILogger<BookingApiController> _logger;

    public BookingApiController(HotelContext context, ILogger<BookingApiController> logger) // Oppdater konstruktøren
    {
        _context = context;
        _logger = logger; // Sett logger-feltet
    }


    // Hente alle reservasjoner
    [HttpGet]
    public IActionResult GetBookings()
    {
        var bookings = _context.Bookings.ToList();
        return Ok(bookings);
    }

    // Legge til en ny reservasjon
    [HttpPost]
    public async Task<ActionResult<Booking>> AddBooking(Booking booking)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); // Returner valideringsfeil
        }

        try
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // Oppdater romtilgjengelighet til false
            var roomToUpdate = await _context.Rooms.FindAsync(booking.RoomId);
            if (roomToUpdate != null)
            {
                roomToUpdate.IsAvailable = false;
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(GetBookings), new { id = booking.Id }, booking);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Feil ved lagring av reservasjon: {ex}");
            return StatusCode(500, $"Feil ved lagring av reservasjon i databasen: {ex.Message}"); // Returner spesifikk feilmelding (kun for utvikling!)
        }
    }
    private void OpprettRengjøringsOppgave(int roomId, string type)
    {
        var task = new ServiceTask
        {
            RoomId = roomId,
            Type = type, // "Cleaner", "Service", "Maintenance"
            Status = "New",
            Note = "Automatisk oppgave etter utsjekk"
        };

        _context.ServiceTasks.Add(task);
        _context.SaveChanges();
    }

    // Endre en reservasjon
    [HttpPut("{id}")]
    public IActionResult UpdateBooking(int id, [FromBody] Booking updatedBooking)
    {
        var booking = _context.Bookings.Find(id);
        if (booking == null) return NotFound();

        booking.CheckIn = updatedBooking.CheckIn;
        booking.CheckOut = updatedBooking.CheckOut;
        _context.SaveChanges();

        return Ok(booking);
    }

    // Slette en reservasjon
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

        // Oppdater romtilgjengelighet
        var roomToUpdate = await _context.Rooms.FindAsync(roomId);
        if (roomToUpdate != null)
        {
            roomToUpdate.IsAvailable = true;
            await _context.SaveChangesAsync();
        }

        return NoContent();
    }
}
