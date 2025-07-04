using CarRentalHub.API.Data;
using CarRentalHub.API.Models;
using CarRentalHub.API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "User")]
public class UserController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Dashboard()
    {
        var cars = _context.Cars.Where(c => c.IsAvailable).ToList();
        return View(cars);
    }

    [HttpGet]
    public IActionResult Book(int id)
    {
        var car = _context.Cars.FirstOrDefault(c => c.Id == id && c.IsAvailable);
        if (car == null) return NotFound();

        var model = new BookingViewModel
        {
            CarId = car.Id,
            Brand = car.Brand,
            Model = car.Model,
            Color = car.Color,
            PricePerDay = car.PricePerDay,
            BookingDate  = DateTime.Today,
            EndDate = DateTime.Today.AddDays(1)
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Book(BookingViewModel model)
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
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            Console.WriteLine("User is null or not authenticated.");
            return Unauthorized();
        }

        var car = await _context.Cars.FirstOrDefaultAsync(c => c.Id == model.CarId);
        if (car == null) return View(model);

        double distance = await OpenRouteServiceHelper.GetDistanceInKmAsync(model.FromLocation, model.ToLocation);
        decimal calculatedPrice = (decimal)distance * 2;
        decimal totalPrice = car.PricePerDay + calculatedPrice;

        var booking = new Booking
        {
            UserId = user.Id,
            CarId = model.CarId,
            BookingDate  = model.BookingDate ,
            EndDate = model.EndDate,
            FromLocation = model.FromLocation,
            ToLocation = model.ToLocation,
            Distance = distance,
            CalculatedPrice = calculatedPrice,
            TotalPrice = totalPrice
        };

        _context.Bookings.Add(booking);

        if (car != null)
        {
            car.IsAvailable = false;
        }

        await _context.SaveChangesAsync();
        Console.WriteLine($"Booking saved: UserId={booking.UserId}, CarId={booking.CarId}, BookingDate={booking.BookingDate}, EndDate={booking.EndDate}, Distance={booking.Distance}, CalculatedPrice={booking.CalculatedPrice}, TotalPrice={booking.TotalPrice}");

        return RedirectToAction("BookingConfirmation", new { id = booking.Id });
    }

    [HttpGet]
    public IActionResult BookingConfirmation(int id)
    {
        var booking = _context.Bookings
                              .Include(b => b.Car)
                              .FirstOrDefault(b => b.Id == id);

        if (booking == null) return NotFound();

        return View(booking);
    }

    [HttpGet]
    public async Task<IActionResult> MyBookings()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();
        var bookings = _context.Bookings.Include(b => b.Car).Where(b => b.UserId == user.Id).ToList();
        return View(bookings);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelBooking(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();
        var booking = _context.Bookings.Include(b => b.Car).FirstOrDefault(b => b.Id == id && b.UserId == user.Id);
        if (booking == null || booking.Canceled) return NotFound();
        booking.Canceled = true;
        if (booking.Car != null)
        {
            booking.Car.IsAvailable = true;
        }
        await _context.SaveChangesAsync();
        return RedirectToAction("MyBookings");
    }

    [HttpPost]
    public async Task<IActionResult> CalculateBookingPrice(string fromLocation, string toLocation, int carId, string bookingDate, string endDate)
    {
        var car = await _context.Cars.FirstOrDefaultAsync(c => c.Id == carId);
        if (car == null) return Json(new { success = false, message = "Car not found" });
        double distance = await OpenRouteServiceHelper.GetDistanceInKmAsync(fromLocation, toLocation);
        decimal calculatedPrice = (decimal)distance * 2;
        decimal total = car.PricePerDay + calculatedPrice;
        return Json(new { success = true, distance, calculatedPrice, pricePerDay = car.PricePerDay, total });
    }
}
