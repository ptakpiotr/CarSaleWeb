using CarSale.Data.MyAttributes;
using CarSale.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CarSale.Data
{
    public class CarsDbContext : DbContext
    {
        public CarsDbContext(DbContextOptions<CarsDbContext> opts) : base(opts)
        {
             
        }

        public DbSet<CarModel> Cars { get; set; }
    }
}
