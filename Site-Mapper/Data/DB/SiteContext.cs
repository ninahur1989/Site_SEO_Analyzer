namespace Site_Mapper.Data.DB
{
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json.Linq;
    using Site_Mapper.Data.Models;

    internal class SiteContext : DbContext
    {
        public SiteContext() : base()
        {
        }

        public DbSet<SiteInfo> SiteInfos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer((string)JObject.Parse(File.ReadAllText(Path.GetFullPath(@"..\..\..\appsettings.json")))["DefaultConnectionString"]);
        }
    }
}
