using System.Collections.Generic;
using System.Linq;
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

        // GET: api/tasks?type=Cleaner
        [HttpGet]
        public ActionResult<IEnumerable<ServiceTask>> GetTasks(string? type = null)
        {
            var query = _context.ServiceTasks.AsQueryable();

            if (!string.IsNullOrEmpty(type))
                query = query.Where(t => t.Type == type && t.Status != "Finished");

            return Ok(query.ToList());
        }

        // PUT: api/tasks/5
        [HttpPut("{id}")]
        public IActionResult UpdateTask(int id, [FromBody] ServiceTask updated)
        {
            var task = _context.ServiceTasks.Find(id);
            if (task == null) return NotFound();

            task.Status = updated.Status;
            task.Note = updated.Note;
            _context.SaveChanges();

            return NoContent();
        }

        // POST: api/tasks
        [HttpPost]
        public IActionResult Create([FromBody] ServiceTask task)
        {
            if (!_context.Rooms.Any(r => r.Id == task.RoomId))
                return BadRequest($"Room with ID {task.RoomId} does not exist.");

            task.Status = "New"; // optional default status
            _context.ServiceTasks.Add(task);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetTasks), new { id = task.Id }, task);
        }
    }
}
