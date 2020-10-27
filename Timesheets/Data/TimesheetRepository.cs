using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Timesheets.Data
{
    public class TimesheetRepository : ITimesheetRepository
    {
        private readonly string _connectionString;
        public TimesheetRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void CreateTimesheet(Timesheet timesheet)
        {
            using (var conn = OpenConnection())
            {
                var id = conn.ExecuteScalar<int>(@"
INSERT INTO timesheet(userid,date,starttime,endtime,comment,absence)
VALUES(@UserId,@Date,@StartTime,@EndTime,@Comment,@Absence);

SELECT LAST_INSERT_ID();
", timesheet);

                timesheet.Id = id;
            }
        }

        public void DeleteTimesheet(int id)
        {
            using (var conn = OpenConnection())
            {
                conn.Execute("DELETE FROM timesheet WHERE id=@id", new { id });
            }
        }

        public Timesheet GetTimesheet(int id)
        {
            using (var conn = OpenConnection())
            {
                return conn.QuerySingleOrDefault<Timesheet>("SELECT * FROM timesheet WHERE id=@id", new { id });
            }
        }

        public Timesheet GetTimesheet(int userId, DateTime date)
        {
            using (var conn = OpenConnection())
            {
                return conn.QuerySingleOrDefault<Timesheet>("SELECT * FROM timesheet WHERE userid=@userId AND date=@date", new
                {
                    userId,
                    date.Date
                });
            }
        }

        public IEnumerable<Timesheet> GetTimesheets(int userId, DateTime startDate, DateTime endDate)
        {
            using (var conn = OpenConnection())
            {
                return conn.Query<Timesheet>("SELECT * FROM timesheet WHERE userid = @userId AND date >= @startDate AND date <= @endDate", new
                {
                    userId,
                    startDate = startDate.Date,
                    endDate = endDate.Date
                }).ToArray();
            }
        }

        public IEnumerable<UserTimesheet> GetTimesheets(DateTime startDate, DateTime endDate)
        {
            using (var conn = OpenConnection())
            {
                return conn.Query<UserTimesheet>(@"SELECT timesheet.*, user.displayname FROM timesheet 
JOIN aspnetusers user ON user.id=timesheet.userid
WHERE timesheet.date >= @startDate AND timesheet.date <= @endDate", new
                {
                    startDate = startDate.Date,
                    endDate = endDate.Date
                }).ToArray();
            }
        }

        public void UpdateTimesheet(Timesheet timesheet)
        {
            using (var conn = OpenConnection())
            {
                conn.Execute(@"UPDATE timesheet SET 
date=@Date, 
starttime=@StartTime, 
endtime=@EndTime, 
comment=@Comment, 
absence=@Absence 
WHERE id=@Id", timesheet);
            }
        }

        private DbConnection OpenConnection()
        {
            var conn = new MySqlConnection(_connectionString);
            conn.Open();
            return conn;
        }
    }
}
