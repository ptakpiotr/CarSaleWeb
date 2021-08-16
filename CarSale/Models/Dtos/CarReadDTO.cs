using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarSale.Models.Dtos
{
    public class CarReadDTO
    {
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public string Make { get; set; }

        public string Model { get; set; }

        public int Year { get; set; }

        public List<string> Photos { get; set; }

        public string Description { get; set; }

        public int Mileage { get; set; }

        public int Price { get; set; }
    }
}
