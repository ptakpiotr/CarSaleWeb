using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CarSale.Models
{
    public class DeleteCarModel
    {
        [Required]
        public string CarId { get; set; }

        [Required]
        [Compare(nameof(CarId),ErrorMessage="The ids are not identical")]
        [Display(Name ="Confirm car id")]
        public string ConfirmCarId { get; set; }
    }
}
