namespace Site_Mapper
{
    using Ninject.Modules;
    using Site_Mapper.Services;
    using Site_Mapper.Services.Interfaces;

    public class Bindings : NinjectModule
    {
        public override void Load()
        {
            Bind<IUrlService>().To<UrlService>();
            Bind<IWatchService>().To<WatchService>();
            Bind<IFileService>().To<FileService>();
        }
    }
}
