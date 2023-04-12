namespace Site_Mapper.Services
{
    using Site_Mapper.Services.Interfaces;
    using Site_Mapper.Services.Logger;

    internal class WatchService : IWatchService
    {
        public async Task<Dictionary<string, long>> WatchRequestAsync(HashSet<string> list)
        {
            var client = new HttpClient();
            var linkTimes = new Dictionary<string, long>();

            foreach (var link in list)
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();

                try
                {
                    using (var response = await client.GetAsync(link))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            watch.Stop();
                            linkTimes.Add(link, watch.ElapsedMilliseconds);
                        }
                        else
                        {
                            watch.Stop();
                            linkTimes.Add(link, -1);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogsFolder.Logs.Add(ex.Message);
                    linkTimes.Add(link, -1);
                    continue;
                }
            }

            return linkTimes.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
