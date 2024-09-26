using System.ComponentModel.DataAnnotations;

namespace CarApi.Models
{
    public class CarMake
    {
        public int CarMakeId { get; set; }

        public string? CarMakeName { get; set; }

        public bool ActiveFlag { get; set; }
    }
}
