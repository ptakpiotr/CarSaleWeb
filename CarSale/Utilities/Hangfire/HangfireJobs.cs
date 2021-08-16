using CarSale.Data;
using Hangfire;
using System;
using System.Linq;

namespace CarSale.Utilities.Hangfire
{
    public class HangfireJobs
    {
        private readonly ICarsRepo _cars;

        public HangfireJobs(ICarsRepo cars)
        {
            _cars = cars;
        }

        public void DeleteOldCars()
        {
            var oldCars = _cars.GetAllCars().Where(c => c.Date < DateTime.Now.AddDays(-7)).ToList();

            _cars.DeleteOldCars(oldCars);
            _cars.SaveChanges();
        }

        public void CallHangfireDeleteOld()
        {
            RecurringJob.AddOrUpdate("DeleteOldCars", () => DeleteOldCars(), Cron.Weekly);
        }

    }
}
