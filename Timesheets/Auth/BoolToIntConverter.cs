using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Timesheets.Auth
{
    public class BoolToIntConverter : ValueConverter<bool,int>
    {
        public BoolToIntConverter() : base(
            b => Convert.ToInt32(b),
            i => Convert.ToBoolean(i))
        {

        }
    }
}
