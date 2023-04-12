namespace Site_Mapper
{
    using Ninject;
    using System.Reflection;
    using Site_Mapper.Services.Interfaces;
    using Site_Mapper.Data.DB;
    using Newtonsoft.Json.Linq;

    internal class Program
    {
        static void Main(string[] args)
        {

            AppDBInitializer.Initialize();
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());

            var urlService = kernel.Get<IUrlService>();
            var watchService = kernel.Get<IWatchService>();


            var formHandler = new Startup(urlService, watchService);
            formHandler.Start();
        }
    }
}
