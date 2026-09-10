using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SamletInfo.Data;
using SamletInfo.Models;

namespace SamletInfo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HotelContext _context;

        public HomeController(ILogger<HomeController> logger, HotelContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [Route("Home")]
        [Route("Home/Index")]
        public IActionResult Index()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var bookings = new List<Booking>();

            if (!string.IsNullOrEmpty(userEmail))
            {
                bookings = _context.Bookings
                    .Include(b => b.Room)
                    .Where(b => b.CustomerEmail == userEmail)
                    .OrderByDescending(b => b.CheckIn)
                    .ToList();
            }

            return View(bookings);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
