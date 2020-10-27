using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Timesheets.Data
{
    public interface ITimesheetRepository
    {
        void CreateTimesheet(Timesheet timesheet);
        void UpdateTimesheet(Timesheet timesheet);
        Timesheet GetTimesheet(int id);
        Timesheet GetTimesheet(int userId, DateTime date);
        IEnumerable<Timesheet> GetTimesheets(int userId, DateTime startDate, DateTime endDate);
        IEnumerable<UserTimesheet> GetTimesheets(DateTime startDate, DateTime endDate);
        void DeleteTimesheet(int id);
    }
}
