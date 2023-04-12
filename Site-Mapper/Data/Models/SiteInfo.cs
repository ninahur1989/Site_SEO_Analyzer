namespace Site_Mapper.Data.Models
{
    internal class SiteInfo
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int CountUrls { get; set; }

        public int CountSiteMapUrls { get; set; }

        public int TotalUrlCount { get; set; }

        public DateTime Date {  get; set; } 

    }
}
