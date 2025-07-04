using System.ComponentModel.DataAnnotations;

namespace CarRentalHub.API.ViewModels
{
    public class BookingViewModel
    {
        public int CarId { get; set; }

        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? Color { get; set; }

        public decimal PricePerDay { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        public string FromLocation { get; set; }
        [Required]
        public string ToLocation { get; set; }

        public double Distance { get; set; }
        public decimal CalculatedPrice { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
