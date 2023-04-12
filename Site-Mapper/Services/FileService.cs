namespace Site_Mapper.Services
{
    using Newtonsoft.Json.Linq;
    using Site_Mapper.Services.Interfaces;
    using System.IO;

    internal class FileService : IFileService
    {
        public void WriteToFile(List<string> items)
        {
            try
            {
                if (items.Count() <= 0)
                    return;

                string path = (string)JObject.Parse(File.ReadAllText(Path.GetFullPath(@"..\..\..\appsettings.json")))["LogsFolder"] + Guid.NewGuid() + "-" + DateTime.UtcNow.ToString("yyyy-MM-dd-hh-mm-ss");
                using (FileStream fs = File.Create(path))
                { }

                using (StreamWriter writer = new StreamWriter(path))
                {
                    foreach (string item in items)
                        writer.WriteLine(item);
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter writer = new StreamWriter(DateTime.UtcNow.ToString()))
                {
                    writer.WriteLine(ex.Message);
                }
            }
        }
    }
}
