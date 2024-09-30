using Dapper;
using System.Data.SqlClient;
using System.Collections.Generic;
using CarApi.Models;

namespace CarApi.Repositories
{
    public class CarRepository
    {
        private readonly string _connectionString;

        public CarRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<CarBrand> GetActiveCarBrands()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = "SELECT * FROM CarBrands WHERE ActiveFlag = 1";
                return connection.Query<CarBrand>(sql);
            }
        }

        public IEnumerable<CarModel> GetCarModelsByBrandId(int carBrandId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = @"
                    SELECT cbcm.CarMakeId, cbcm.CarBrandId,
                           cb.BrandName AS CarBrand,
                           cm.MakeName AS CarMake,
                           cb.ActiveFlag, cm.ActiveFlag
                    FROM CarBrandCarMakeMatrix cbcm
                    JOIN CarBrands cb ON cb.CarBrandId = cbcm.CarBrandId
                    JOIN CarMake cm ON cm.CarMakeId = cbcm.CarMakeId
                    WHERE cbcm.CarBrandId = @CarBrandId
                    AND cb.ActiveFlag = 1
                    AND cm.ActiveFlag = 1";
                return connection.Query<CarModel>(sql, new { CarBrandId = carBrandId });
            }
        }

        public (int CarBrandId, int CarMakeId)? GetCarBrandCarMake(string brandName, string makeName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = @"
                    SELECT cbcm.CarBrandId, cbcm.CarMakeId
                    FROM CarBrandCarMakeMatrix cbcm
                    JOIN CarBrands cb ON cb.CarBrandId = cbcm.CarBrandId
                    JOIN CarMake cm ON cm.CarMakeId = cbcm.CarMakeId
                    WHERE cb.BrandName = @BrandName
                    AND cm.MakeName = @MakeName
                    AND cb.ActiveFlag = 1
                    AND cm.ActiveFlag = 1";
                return connection.QueryFirstOrDefault<(int CarBrandId, int CarMakeId)>(sql, new { BrandName = brandName, MakeName = makeName });
            }
        }

        public int InsertSubmit(int carBrandId, int carMakeId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = @"
                    INSERT INTO Submit (CarBrandId, CarMakeId, CreatedAt)
                    VALUES (@CarBrandId, @CarMakeId, GETDATE());";
                return connection.Execute(sql, new { CarBrandId = carBrandId, CarMakeId = carMakeId });
            }
        }

        public int InsertDisplay(string carBrand, string carMake)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = "INSERT INTO DisplayTable(CarBrand, CarMake) VALUES(@CarBrand, @CarMake)";
                return connection.Execute(sql, new { CarBrand = carBrand, CarMake = carMake });
            }
        }
    }
}
