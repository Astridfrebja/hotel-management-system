using Microsoft.AspNetCore.Mvc;
using SamletInfo.Data;
using SamletInfo.Models;

namespace SamletInfo.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class ServiceTaskController : ControllerBase
    {
        private readonly HotelContext _context;

        public ServiceTaskController(HotelContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ServiceTask>> GetTasks(string? type = null, bool all = false)
        {
            var query = _context.ServiceTasks.AsQueryable();

            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(t => t.Type == type);
            }

            if (!all)
            {
                query = query.Where(t => t.Status != "Finished" && t.Status != "Done");
            }

            return Ok(query.OrderBy(t => t.RoomId).ToList());
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTask(int id, [FromBody] ServiceTask updated)
        {
            var task = _context.ServiceTasks.Find(id);
            if (task == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(updated.Status))
            {
                task.Status = updated.Status;
            }

            if (updated.Note != null)
            {
                task.Note = updated.Note;
            }

            if (task.Status is "Finished" or "Done" && task.Type == "Cleaner")
            {
                var stillOccupied = _context.Bookings.Any(b =>
                    b.RoomId == task.RoomId && b.Status == "CheckedIn");
                var room = _context.Rooms.Find(task.RoomId);
                if (room != null && !stillOccupied)
                {
                    room.IsAvailable = true;
                }
            }

            _context.SaveChanges();
            return Ok(task);
        }

        [HttpPost]
        public IActionResult Create([FromBody] ServiceTask task)
        {
            if (!_context.Rooms.Any(r => r.Id == task.RoomId))
            {
                return BadRequest($"Room with ID {task.RoomId} does not exist.");
            }

            task.Status = string.IsNullOrWhiteSpace(task.Status) ? "New" : task.Status;
            _context.ServiceTasks.Add(task);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetTasks), new { id = task.Id }, task);
        }
    }
}
