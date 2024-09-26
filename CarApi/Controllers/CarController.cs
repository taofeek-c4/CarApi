using CarApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data.SqlTypes;
    
namespace CarApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly string connectionString ;   
        public CarController(IConfiguration configuration )
        {
            connectionString = configuration["ConnectionStrings:SqlServerDB"] ?? "";
        }

        // For the car brands selection
        [HttpGet("CarBrands")]
        public IActionResult GetCarBrand()
        {
            List<CarBrand> carBrands = new List<CarBrand>();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT * FROM CarBrands WHERE ActiveFlag = 1";
                    using (var Command = new SqlCommand(sql, connection))
                    {
                        using (var reader = Command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CarBrand carBrand = new CarBrand();
                                carBrand.CarBrandId = reader.GetInt32(0);
                                carBrand.BrandName = reader.GetString(1);
                                carBrand.ActiveFlag = reader.GetBoolean(2);
                                carBrands.Add(carBrand);    
                            }
                        }
                    }
                }
                return Ok(carBrands);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("carBrand", $"An error occured,{ex}");
                return BadRequest(ModelState);
            }
        }

    //// For the car models selection
    [HttpGet("CarModels")]
    public IActionResult GetCarModels([FromQuery]int id)
    {
        List<CarBrandCarMakeMatrix> carBrandCarMakeMatrix = new List<CarBrandCarMakeMatrix>();
    
        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
            
                string sql = @"SELECT cbcm.CarMakeId, cbcm.CarBrandId, 
                                      cb.BrandName AS CarBrand, 
                                      cm.MakeName AS CarMake 
                               FROM CarBrandCarMakeMatrix cbcm 
                               JOIN CarBrands cb ON cb.CarBrandId = cbcm.CarBrandId 
                               JOIN CarMake cm ON cm.CarMakeId = cbcm.CarMakeId 
                               WHERE cb.CarBrandId = @CarBrandId 
                               AND cb.ActiveFlag = 1 
                               AND cm.ActiveFlag = 1";
                using (var Command = new SqlCommand(sql, connection))
                {
                    Command.Parameters.AddWithValue("@CarBrandId", id);

                    using (var reader = Command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CarBrandCarMakeMatrix carBrand = new CarBrandCarMakeMatrix();

                            carBrand.CarMakeId  = reader.GetInt32(0); 
                            carBrand.CarBrandId = reader.GetInt32(1); 
                            carBrand.CarBrandName = reader.GetString(2); 
                            carBrand.CarMakeName    = reader.GetString(3);  

                            carBrandCarMakeMatrix.Add(carBrand);
                        }
                    }
                }
            }   
            return Ok(carBrandCarMakeMatrix);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("carMake", $"An error occurred: {ex.Message}");
            return BadRequest(ModelState);
        }
    }

        // For the sumbit button
        [HttpPost("Submit")]
        public IActionResult Submit(Submit submit)
        {
            try
            {
                if (submit == null)
                {
                    ModelState.AddModelError("Submit", "Invalid CarBrand or CarMake.");
                    return BadRequest(ModelState);
                }
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "INSERT INTO Submit" +
                                 "(CarBrandId, CarMakeId)" +
                                 "VALUES (@CarBrand, @CarMake)";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@CarBrand", submit.CarBrandId);
                        command.Parameters.AddWithValue("@CarMake", submit.CarMakeId);
                        command.ExecuteNonQuery();
                    }
                }
                return Ok(new { Message = "Submission successful", Data = submit });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Product", $"Sorry, an error occurred: {ex.Message}");
                return BadRequest(ModelState);
            }
        }

    }
}
