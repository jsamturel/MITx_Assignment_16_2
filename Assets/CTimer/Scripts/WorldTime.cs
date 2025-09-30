using System;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

namespace CTools.CTimer
{
    public class WorldTime : IDisposable
    {
        private const string URL = "https://timeapi.io/api/time/current/zone?timeZone=UTC";

        public bool IsTimeFetchSuccessfull { get; private set; }

        // UTC Time data saved after fetched
        public DateTime? FetchedUTCTime { get; private set; }

        // Current UTC time calculated
        public DateTime? UTCTime
        {
            get
            {
                return FetchedUTCTime.HasValue
                    ? FetchedUTCTime.Value.AddSeconds(Time.time)
                    : null;
            }
        }

        public event Action<WorldTime , DateTime> OnTimeFetchComplete;

        public WorldTime()
        {
            _ = TryFetchWorldTime();
        }

        public void Dispose()
        {
            OnTimeFetchComplete = null;
        }

        private async Task TryFetchWorldTime()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(URL);

                    response.EnsureSuccessStatusCode();

                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    TimeAPIResponse timeAPIResponse = JsonUtility.FromJson<TimeAPIResponse>(jsonResponse);

                    if (DateTime.TryParse(timeAPIResponse.dateTime , out DateTime utcTime))
                    {
                        FetchedUTCTime = utcTime;

                        IsTimeFetchSuccessfull = true;
                        OnTimeFetchComplete?.Invoke(this , UTCTime.Value);
                    }
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.LogWarning($"Couldn't fecth world time : {e}");

                    throw;
                }
            }
        }
    }
}