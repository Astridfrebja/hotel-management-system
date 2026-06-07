using Microsoft.AspNetCore.Mvc;
using SamletInfo.Data;
using SamletInfo.Models;
using System.Collections.Generic;
using System.Linq;

namespace SamletInfo.Controllers
{
    [ApiController]
    [Route("api/tasktemplates")]
    public class TaskTemplateController : ControllerBase
    {
        private readonly HotelContext _context;

        public TaskTemplateController(HotelContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<List<TaskTemplate>> GetAll()
        {
            return Ok(_context.TaskTemplates.ToList());
        }

        [HttpGet("{type}")]
        public ActionResult<List<TaskTemplate>> GetByType(string type)
        {
            var filtered = _context.TaskTemplates
                .Where(t => t.Type == type)
                .ToList();

            return Ok(filtered);
        }
    }
}
