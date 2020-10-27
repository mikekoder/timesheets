using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace Timesheets.Mgmt
{
    public static class UpdateEndTime
    {
        [FunctionName("UpdateEndTime")]
        public static void Run([TimerTrigger("0 5 0 * * *")]TimerInfo myTimer, ILogger log)
        {
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            var connectionString = config["connectionstring"];
            using (var conn = new MySqlConnection(connectionString))
            using(var cmd = new MySqlCommand("UPDATE timesheet SET endtime=@endTime WHERE date <= @date AND endtime IS NULL", conn))
            {
                cmd.Parameters.AddWithValue("endTime", new TimeSpan(16, 0, 0));
                cmd.Parameters.AddWithValue("date", DateTime.Today.AddDays(-1));
                conn.Open();
                var rowCount = cmd.ExecuteNonQuery();
                log.LogInformation(rowCount + " rows updated");
            }

            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
