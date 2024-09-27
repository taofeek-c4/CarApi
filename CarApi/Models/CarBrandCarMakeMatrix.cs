using System.ComponentModel.DataAnnotations;

namespace CarApi.Models
{
    public class CarBrandCarMakeMatrix
    {
        public int CarBrandId { get; set; }

        [Required(ErrorMessage = "CarBrandName is required.")]
        public string CarBrandName { get; set; }

        public int CarMakeId { get; set; }

        [Required(ErrorMessage = "CarMakeName is required.")]
        public string CarMakeName { get;  set; }
    }
}
