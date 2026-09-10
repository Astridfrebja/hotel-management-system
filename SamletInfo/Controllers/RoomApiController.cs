using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SamletInfo.Data;
using SamletInfo.Models;
using SamletInfo.Services; 

namespace SamletInfo.Controllers
{
    [ApiController]
    [Route("api/rooms")]
   
    public class RoomApiController : ControllerBase
    {
        private readonly HotelContext _context;

        public RoomApiController(HotelContext context)
        {
            _context = context;
        }

        // Hente alle rom
        [HttpGet]
        public IActionResult GetRooms(DateTime? from = null, DateTime? to = null, int? beds = null, string? quality = null)
        {
            var rooms = _context.Rooms.AsQueryable();
            if (beds != null)
            {
                rooms = rooms.Where(r => r.Beds == beds);
            }

            if (!string.IsNullOrEmpty(quality))
            {
                rooms = rooms.Where(r => r.Quality == quality);
            }

            var list = rooms.ToList();
            if (from != null && to != null)
            {
                list = list.Where(r => RoomAvailability.IsFree(_context, r.Id, from.Value, to.Value)).ToList();
            }

            return Ok(list);
        }
    }
}
