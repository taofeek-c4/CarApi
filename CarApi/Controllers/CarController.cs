using CarApi.Models;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace CarApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly string connectionString;
        public CarController(IConfiguration configuration)
        {
            connectionString = configuration["ConnectionStrings:SqlServerDB"] ?? "";
        }
        
        // This guy uses Dapper to fetch the carbrands from the database
        [HttpGet("CarBrands")]
        public IActionResult GetCarBrand()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT * FROM CarBrands WHERE ActiveFlag = 1";

                    var carBrands = connection.Query<CarBrand>(sql).ToList();

                    return Ok(carBrands);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("carBrand", $"An error occurred: {ex.Message}");
                return BadRequest(ModelState);
            }
        }

        // This guy uses dapper to fetch the carmodels from the database
        [HttpGet("CarModels")]
        public  IActionResult GetCarModels([FromQuery] string CarBrand)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                     connection.Open();

                    var sql = @"SELECT
                                      cbcm.CarMakeId,
                                      cbcm.CarBrandId,
                                      cb.BrandName AS CarBrand, 
                                      cm.MakeName AS CarMake
                                    FROM CarBrandCarMakeMatrix cbcm
                                    JOIN CarBrands cb ON cb.CarBrandId = cbcm.CarBrandId
                                    JOIN CarMake cm ON cm.CarMakeId = cbcm.CarMakeId
                                    WHERE cb.BrandName = @CarBrand
                                    AND cb.ActiveFlag = 1
                                    AND cm.ActiveFlag = 1";

                    var carBrandCarMakeMatrix = connection.Query<object>(sql, new { CarBrand }).ToList();

                    if (carBrandCarMakeMatrix.Count == 0)
                    {
                        return NotFound(new { Message = "No car models found for the selected brand or the brand is inactive." });
                    }

                    return Ok(carBrandCarMakeMatrix);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("carMake", $"An error occurred: {ex.Message}");
                return BadRequest(ModelState);
            }
        }

        // This guy uses dapper to insert the userinput(carbrands and carmodels) in the submit from the database
        [HttpPost ("Submit")]
        public async Task<IActionResult> SubmitClick([FromBody] Submit submit)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    var sql = @"INSERT INTO Submit (CarBrandId, CarMakeId)
                        SELECT cbcm.CarBrandId, cbcm.CarMakeId
                        FROM CarBrandCarMakeMatrix cbcm
                        JOIN CarBrands cb ON cb.CarBrandId = cbcm.CarBrandId
                        JOIN CarMake cm ON cm.CarMakeId = cbcm.CarMakeId
                        WHERE cb.BrandName = @BrandName
                        AND cm.MakeName = @MakeName
                        AND cb.ActiveFlag = 1
                        AND cm.ActiveFlag = 1;";

                    var parameters = new DynamicParameters();
                    parameters.Add("@BrandName", submit.BrandName);
                    parameters.Add("@MakeName", submit.MakeName);

                    var rowsAffected = await connection.ExecuteAsync(sql, parameters);

                    if (rowsAffected > 0)
                    {
                        return Ok("Submission successful.");
                    }
                    else
                    {
                        return NotFound("No matching car brand or make found.");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }
    }
}