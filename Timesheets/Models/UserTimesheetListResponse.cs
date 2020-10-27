using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Timesheets.Models
{
    public class UserTimesheetListResponse
    {
        public string DisplayName { get; set; }
        public TimeSpan FlexTime { get; set; }
        public TimesheetResponse[] Timesheets { get; set; }
    }
}
