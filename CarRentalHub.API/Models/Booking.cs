using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRentalHub.API.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        public int CarId { get; set; }

        [ForeignKey("CarId")]
        public Car Car { get; set; }

        public DateTime BookingDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool Canceled { get; set; } = false;

        [Required]
        public string FromLocation { get; set; }

        [Required]
        public string ToLocation { get; set; }

        public double Distance { get; set; }
        public decimal CalculatedPrice { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
