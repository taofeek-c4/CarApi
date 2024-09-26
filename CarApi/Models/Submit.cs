using System.ComponentModel.DataAnnotations;

namespace CarApi.Models
{
    public class Submit
    {
        [Required]
        public int  CarBrandId { get; set; }
        [Required]
        public int  CarMakeId { get; set; } 
    }
}
