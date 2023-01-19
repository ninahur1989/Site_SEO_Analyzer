namespace Site_Mapper.Services.Interfaces
{
    internal interface IWatchService
    {
        public Task<Dictionary<string, long>> WatchRequestAsync(HashSet<string> list);
    }
}
