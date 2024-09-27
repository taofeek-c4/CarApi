using System.ComponentModel.DataAnnotations;

namespace CarApi.Models
{
    public class Submit
    {
        [Required]
        public int  CarBrandId { get; set; }

        public string? BrandName { get; set; }

        [Required]
        public int  CarMakeId { get; set; }

        public string? MakeName { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
