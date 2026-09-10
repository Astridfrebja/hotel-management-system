using Microsoft.AspNetCore.Mvc;
using SamletInfo.Data;
using SamletInfo.Models;

namespace SamletInfo.Controllers
{
    public class ServiceDeskController : Controller
    {
        private readonly HotelContext _context;

        public ServiceDeskController(HotelContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("ServiceDesk")]
        [Route("ServiceDesk/Index")]
        public IActionResult Index(string? type)
        {
            var mapped = type == "Cleaning" ? "Cleaner" : type;
            var tasks = _context.ServiceTasks.AsQueryable();
            if (!string.IsNullOrEmpty(mapped))
            {
                tasks = tasks.Where(t => t.Type == mapped);
            }

            tasks = tasks.Where(t => t.Status != "Done" && t.Status != "Finished");
            ViewBag.SelectedType = type ?? "";
            return View(tasks.OrderBy(t => t.RoomId).ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Complete(int id, string? type)
        {
            var task = _context.ServiceTasks.Find(id);
            if (task != null)
            {
                task.Status = "Done";
                _context.SaveChanges();
                TempData["Message"] = "Oppgave markert som ferdig.";
            }

            return RedirectToAction(nameof(Index), new { type });
        }
    }
}
