namespace Site_Mapper.Services.Logger
{
    using Ninject;
    using Site_Mapper.Services.Interfaces;
    using System.Reflection;

    internal class Logger
    {
        private readonly IFileService _fileService;

        public Logger()
        {
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());

            _fileService = kernel.Get<IFileService>();
        }


        public void WriteLogs()
        {
            _fileService.WriteToFile(LogsFolder.Logs);
            LogsFolder.Logs = new List<string>();
        }
    }
}
