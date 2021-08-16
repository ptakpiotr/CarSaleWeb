using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CarSale.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string City { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfCreation { get; set; }
    }
}
