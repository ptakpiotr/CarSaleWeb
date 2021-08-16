using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CarSale.Data.MyAttributes
{
    public class YearAttribute : ValidationAttribute
    {
        private readonly int _start;

        public YearAttribute(int start)
        {
            _start = start;
        }

        public override bool IsValid(object value)
        {
            int y = Convert.ToInt32(value);
            return y>=_start && y<=DateTime.Now.Year;
        }
    }
}
