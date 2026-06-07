using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SamletInfo.Data;
using SamletInfo.Models; 

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
        public IActionResult GetRooms()
        {
            var rooms = _context.Rooms.ToList();
            return Ok(rooms);
        }
    }
}
