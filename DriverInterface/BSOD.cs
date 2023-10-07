using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KsDumper11
{
    public class BSOD
    {
        public static bool JustHappened()
        {
            List<DateTime> detectedCrashTimes = new List<DateTime>();

            string eventLogName = "System";

            EventLog eventLog = new EventLog();
            eventLog.Log = eventLogName;

            foreach (EventLogEntry log in eventLog.Entries)
            {
                if (log.EventID == 1001)
                {
                    detectedCrashTimes.Add(log.TimeGenerated);
                }
            }

            detectedCrashTimes = detectedCrashTimes.OrderByDescending(x => x).ToList();

            foreach (DateTime crashTime in detectedCrashTimes)
            {
                if (CheckIfWithinFiveMinutes(crashTime, 5))
                {
                    return true;
                }
            }

            return false;
        }

        static bool CheckIfWithinFiveMinutes(DateTime dateTimeToCheck, int minutesAgo)
        {
            // Get the current time
            DateTime currentTime = DateTime.Now;

            // Calculate the time difference
            TimeSpan timeDifference = currentTime - dateTimeToCheck;

            // Check if the time difference is within 5 minutes
            if (timeDifference.TotalMinutes <= minutesAgo)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
