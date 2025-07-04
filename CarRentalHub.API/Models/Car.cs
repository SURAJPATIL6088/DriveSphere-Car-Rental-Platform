namespace CarRentalHub.API.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Car
    {
        public int Id { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? Color { get; set; }
        [Display(Name = "Price Per Day")]
        public int PricePerDay { get; set; }
        [Display(Name = "Is Available")] 
        public bool IsAvailable { get; set; } = true;
        public string? ImagePath { get; set; }
        [Required]
        [Display(Name = "Driver Name")]
        public string DriverName { get; set; }
        [Required]
        [Display(Name = "Driver Phone")]
        public string DriverPhone { get; set; }
    }
}
