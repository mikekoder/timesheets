using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Timesheets.Auth
{
    public class User : IdentityUser<int>
    {
        public string DisplayName { get; set; }
    }
}
