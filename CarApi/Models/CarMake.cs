using System.ComponentModel.DataAnnotations;

namespace CarApi.Models
{
    public class CarModel
    {
        public int CarMakeId { get; set; }

        public string? CarMake { get; set; }

        public bool ActiveFlag { get; set; }
    }
}
    