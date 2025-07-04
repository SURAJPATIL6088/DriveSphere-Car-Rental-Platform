using CarRentalHub.API.Data;
using CarRentalHub.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace CarRentalHub.API.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            var cars = await _context.Cars.ToListAsync();
            return View(cars);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Car car, IFormFile ImageFile)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is invalid. Errors:");
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"{key}: {error.ErrorMessage}");
                    }
                }
                ViewBag.Error = "Model validation failed. Please check all required fields.";
                return View(car);
            }
            if (ImageFile != null && ImageFile.Length > 0)
            {
                Console.WriteLine($"Image file received: {ImageFile.FileName}, size: {ImageFile.Length}");
                var fileName = Path.GetFileNameWithoutExtension(ImageFile.FileName) + "_" + Guid.NewGuid().ToString().Substring(0, 8) + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine("wwwroot/images/cars", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }
                car.ImagePath = "/images/cars/" + fileName;
            }
            else
            {
                Console.WriteLine("No image file uploaded.");
            }
            try
            {
                _context.Cars.Add(car);
                await _context.SaveChangesAsync();
                Console.WriteLine("Car saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving car: {ex.Message}");
                ViewBag.Error = "Error saving car: " + ex.Message;
                return View(car);
            }
            return RedirectToAction(nameof(Dashboard));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null) return NotFound();
            return View(car);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Car car, IFormFile ImageFile)
        {
            if (!ModelState.IsValid) return View(car);
            var dbCar = await _context.Cars.FindAsync(car.Id);
            if (dbCar == null) return NotFound();
            dbCar.Brand = car.Brand;
            dbCar.Model = car.Model;
            dbCar.Color = car.Color;
            dbCar.PricePerDay = car.PricePerDay;
            dbCar.IsAvailable = car.IsAvailable;
            dbCar.DriverName = car.DriverName;
            dbCar.DriverPhone = car.DriverPhone;
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Path.GetFileNameWithoutExtension(ImageFile.FileName) + "_" + Guid.NewGuid().ToString().Substring(0, 8) + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine("wwwroot/images/cars", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }
                dbCar.ImagePath = "/images/cars/" + fileName;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Dashboard));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null) return NotFound();
            return View(car);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null) return NotFound();

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Dashboard));
        }

        public async Task<IActionResult> Details(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null) return NotFound();
            return View(car);
        }

        public async Task<IActionResult> ListUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> UserBookings(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            var bookings = await _context.Bookings.Include(b => b.Car).Where(b => b.UserId == id).ToListAsync();
            ViewBag.User = user;
            return View(bookings);
        }
    }
}
