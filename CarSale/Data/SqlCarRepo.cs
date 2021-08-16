using CarSale.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarSale.Data
{
    public class SqlCarRepo : ICarsRepo
    {
        private readonly CarsDbContext _context;

        public SqlCarRepo(CarsDbContext context)
        {
            _context = context;
        }

        public void AddCar(CarModel model)
        {
            _context.Cars.Add(model);
        }

        public void DeleteCar(string id)
        {
            var car = _context.Cars.First(c => c.Id == Guid.Parse(id));

            if (car != null)
            {
                _context.Cars.Remove(car);
            }
        }

        public void DeleteOldCars(List<CarModel> old)
        {
            _context.Cars.RemoveRange(old);
        }

        public List<CarModel> GetAllCars()
        {
            var cars = _context.Cars.OrderByDescending(c=>c.Date!=null?c.Date:new DateTime(2000,1,1)).ToList();


            return (cars);
        }

        public List<CarModel> GetCarsByMake(string make)
        {
            var cars = _context.Cars.Where(c => c.Make == make).ToList();

            return (cars);
        }

        public List<CarModel> GetCarsOfUser(string userId)
        {
            var cars = _context.Cars.Where(c => c.UserId == userId).ToList();

            return (cars);
        }

        public CarModel GetSingleCar(string id)
        {
            var car = _context.Cars.First(c => c.Id == Guid.Parse(id));

            if (car == null)
            {
                throw new Exception("No Car with given id");
            }

            return (car);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void UpdateCar(CarModel model)
        {
            //do nothing - AutoMapper does
        }
    }
}
