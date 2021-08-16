using CarSale.Data.MyAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CarSale.Models
{
    public class CarModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string Make { get; set; }

        [Required]
        public string Model { get; set; }

        [Required]
        [Year(1950)]
        public int Year { get; set; }

        [Required]
        public List<string> Photos { get; set; }

        [Required]
        [MinLength(20)]
        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public int Mileage { get; set; }

        [Required]
        public int Price { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }
    }
}
