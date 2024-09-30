using CarApi.Models;
using CarApi.Repositories; 
using Microsoft.AspNetCore.Mvc;

namespace CarApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly CarRepository _carRepository;

        public CarController(IConfiguration configuration)
        {
            var connectionString = configuration["ConnectionStrings:SqlServerDB"] ?? "";
            _carRepository = new CarRepository(connectionString);
        }

        [HttpGet("CarBrands")]
        public IActionResult GetCarBrand()
        {
            try
            {
                var carBrands = _carRepository.GetActiveCarBrands().ToList();
                return Ok(carBrands);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("carBrand", $"An error occurred: {ex.Message}");
                return BadRequest(ModelState);
            }
        }

        [HttpGet("CarModels")]
        public IActionResult GetCarModels([FromQuery] int CarBrandId)
        {
            try
            {
                var carMake = _carRepository.GetCarModelsByBrandId(CarBrandId).ToList();

                if (carMake.Count == 0)
                {
                    return NotFound(new { Message = "No car models found for the selected brand or the brand is inactive." });
                }

                return Ok(carMake);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("carMake", $"An error occurred: {ex.Message}");
                return BadRequest(ModelState);
            }
        }

        [HttpPost("Submit")]
        public IActionResult SubmitClick([FromBody] Submit submit)
        {
            try
            {
                var carBrandCarMake = _carRepository.GetCarBrandCarMake(submit.BrandName, submit.MakeName);
                if (carBrandCarMake == null)
                {
                    return NotFound(new { Message = "Invalid CarBrand or CarMake. Please check your selection." });
                }

                var rowsAffected = _carRepository.InsertSubmit(carBrandCarMake.Value.CarBrandId, carBrandCarMake.Value.CarMakeId);
                if (rowsAffected > 0)
                {
                    return Ok("Submission successful.");
                }
                else
                {
                    return BadRequest("Failed to insert data.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("Display")]
        public IActionResult Display([FromBody] Display display)
        {
            try
            {
                var rowsAffected = _carRepository.InsertDisplay(display.CarBrand, display.CarMake);
                if (rowsAffected > 0)
                {
                    return Ok("Submission successful.");
                }
                else
                {
                    return NotFound("No matching car brand or make found.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("carMake", $"An error occurred: {ex.Message}");
                return BadRequest(ModelState);
            }
        }
    }
}
