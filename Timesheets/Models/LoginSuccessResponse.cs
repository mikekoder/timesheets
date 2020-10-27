using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Timesheets.Models
{
    public class LoginSuccessResponse
    {
        public string AccessToken { get; set; }
        public bool IsManager { get; set; }
    }
}
