using System;
using UnityEngine;

namespace CTools.CTimer
{
    public class UserTimeData
    {
        private const string APP_FIRST_OPEN_SAVEKEY = "ApplicationFirstFetchedTimeData";
        private string AppFirstOpenTimeTicks
        {
            get => PlayerPrefs.GetString(APP_FIRST_OPEN_SAVEKEY , "");
            set => PlayerPrefs.SetString(APP_FIRST_OPEN_SAVEKEY , value);
        }

        // First install date time data
        public DateTime? FirstInstallTime { get; private set; }

        // Returns true if data calculated and initialized successfully
        public bool IsInitialized { get; private set; }

        public WorldTime WorldTime { get; private set; }

        public UserTimeData(WorldTime worldTime)
        {
            this.WorldTime = worldTime;

            worldTime.OnTimeFetchComplete += WorldTime_OnTimeFetchComplete;

            if (worldTime.IsTimeFetchSuccessfull)
                ParseFirstInstallTime(worldTime.UTCTime.Value);
        }

        public TimeSpan? GetPassedTimeSinceFirstInstall()
        {
            if (!WorldTime.UTCTime.HasValue
                || !FirstInstallTime.HasValue)
                return null;

            return WorldTime.UTCTime.Value - FirstInstallTime.Value;
        }

        private bool ParseFirstInstallTime(DateTime utcTime)
        {
            if (string.IsNullOrEmpty(AppFirstOpenTimeTicks)) // No saved data, save first install
            {
                FirstInstallTime = utcTime;

                AppFirstOpenTimeTicks = utcTime.Ticks.ToString();

                return false;
            }
            else // Has saved data, parse the data
            {
                if (long.TryParse(AppFirstOpenTimeTicks , out long ticks))
                {
                    FirstInstallTime = new DateTime(ticks);

                    return true;
                }
            }

            return false;
        }

        private void WorldTime_OnTimeFetchComplete(WorldTime worldTime , DateTime utcTime)
        {
            ParseFirstInstallTime(utcTime);
        }
    }
}