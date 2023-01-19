namespace Site_Mapper
{
    using Ninject;
    using System.Reflection;
    using Site_Mapper.Services.Interfaces;

    internal class Program
    {
        static void Main(string[] args)
        {
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());

            var urlService = kernel.Get<IUrlServise>();
            var watchService = kernel.Get<IWatchService>();

            var formHandler = new Startup(urlService, watchService);
            formHandler.Start();
        }
    }
}
