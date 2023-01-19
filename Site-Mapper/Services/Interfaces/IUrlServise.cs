namespace Site_Mapper.Services.Interfaces
{
    internal interface IUrlServise
    {
        public IEnumerable<string> GetUrls(string url);

        public IEnumerable<string> GetSitemapUrls(string url);
    }
}
