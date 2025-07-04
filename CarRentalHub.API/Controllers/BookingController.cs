using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRentalHub.API.Data;
using CarRentalHub.API.Models;


namespace CarRentalHub.API.Controllers
{
    [Authorize(Roles = "User")]
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("available-cars")]
        public async Task<IActionResult> GetAvailableCars()
        {
            var cars = await _context.Cars
                .Where(c => c.IsAvailable)
                .ToListAsync();

            return Ok(cars);
        }

        [HttpGet("my-bookings")]
        public async Task<IActionResult> MyBookings()
        {
            var user = await _userManager.GetUserAsync(User);
            var bookings = await _context.Bookings
                .Include(b => b.Car)
                .Where(b => b.UserId == user.Id)
                .ToListAsync();

            return Ok(bookings);
        }

        [HttpPost("book")]
        public async Task<IActionResult> BookCar([FromBody] Booking request)
        {
            var user = await _userManager.GetUserAsync(User);

            var car = await _context.Cars.FindAsync(request.CarId);
            if (car == null || !car.IsAvailable)
                return BadRequest("Car is not available");

            var booking = new Booking
            {
                CarId = request.CarId,
                UserId = user.Id,
                BookingDate = request.BookingDate,
                EndDate = request.EndDate
            };

            car.IsAvailable = false;

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Car booked successfully" });
        }
    }
}
