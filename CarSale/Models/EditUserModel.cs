using CarSale.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarSale.Models
{
    public class EditUserModel
    {
        public ReadUserDTO User { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public List<string> Claims { get; set; } = new List<string>();

    }
}
