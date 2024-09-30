using System.ComponentModel.DataAnnotations;

namespace CarApi.Models
{
    public class Submit
    {
        public string ? BrandName { get; set; }

        public string ? MakeName { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
