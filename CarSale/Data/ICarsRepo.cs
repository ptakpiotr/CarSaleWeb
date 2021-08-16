using CarSale.Models;
using System.Collections.Generic;

namespace CarSale.Data
{
    public interface ICarsRepo
    {
        List<CarModel> GetAllCars();
        List<CarModel> GetCarsByMake(string make);
        List<CarModel> GetCarsOfUser(string userId);
        CarModel GetSingleCar(string id);
        void AddCar(CarModel model);
        void UpdateCar(CarModel model);
        void DeleteCar(string id);
        void DeleteOldCars(List<CarModel> old);

        void SaveChanges();
    }
}
