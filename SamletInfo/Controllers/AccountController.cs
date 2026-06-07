using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using SamletInfo.Data;
using SamletInfo.Models;

namespace SamletInfo.Controllers
{
    public class AccountController : Controller
    {
        private readonly HotelContext _context;
        public AccountController(HotelContext context)
        {
            _context = context;
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email address is already registered.");
                    return View(user);
                }
                user.Password = HashPassword(user.Password);

                _context.Users.Add(user);
                _context.SaveChanges();

                return RedirectToAction("Login");
            }
            return View(user);
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string password, string returnUrl = null)
        {
            Console.WriteLine($"Login attempt for email: {email}");

            if (_context == null)
            {
                Console.WriteLine("_context er null!");
                ModelState.AddModelError(string.Empty, "Database context er ikke tilgjengelig.");
                return View();
            }

            var user = _context.Users.SingleOrDefault(u => u.Email == email);

            if (user != null && VerifyPassword(password, user.Password))
            {
                HttpContext.Session.SetString("UserEmail", user.Email);

                // Hvis det finnes en ReturnUrl, redirect dit – ellers til forsiden
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Booking");
                }
            }

            ModelState.AddModelError(string.Empty, "Ugyldig e-post eller passord");
            return View();
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserEmail");
            return RedirectToAction("Index", "Home");
        }
        private string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password!,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 1000,
                numBytesRequested: 256 / 8));
            return $"{Convert.ToBase64String(salt)}:{hashed}";
        }
        private bool VerifyPassword(string providedPassword, string storedHash)
        {
            if (string.IsNullOrEmpty(storedHash))
            {
                Console.WriteLine("Stored hash is null or empty."); 
                return false; // Or throw an exception: throw new ArgumentNullException(nameof(storedHash), "Stored hash cannot be null or empty.");
            }
            try
            {
                Console.WriteLine($"Stored Hash: {storedHash}");
                string[] parts = storedHash.Split(':');
                byte[] salt = Convert.FromBase64String(parts[0]);
                string storedHashedPassword = parts[1];

                Console.WriteLine($"Salt: {Convert.ToBase64String(salt)}");
                Console.WriteLine($"Stored Hashed Password: {storedHashedPassword}"); 

                string hashedProvided = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: providedPassword!,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));

                Console.WriteLine($"Hashed Provided Password: {hashedProvided}");
                return storedHashedPassword == hashedProvided;
            }
            catch (Exception ex)
            {
                // Log the exception!  This is crucial for debugging.
                Console.WriteLine($"Error in VerifyPassword: {ex.Message}");
                return false; // Consider different return value or throw, depending on your error handling policy
            }
        }
    }
}
