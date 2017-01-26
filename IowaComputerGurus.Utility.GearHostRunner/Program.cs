using IowaComputerGurus.Utility.GearHost.Jobs;
using IowaComputerGurus.Utility.GearHost.Providers;
using IowaComputerGurus.Utility.GearHost.Services;
using log4net;
using log4net.Config;
using Ninject;
using Ninject.Activation;

namespace IowaComputerGurus.Utility.GearHostRunner
{
    internal class Program
    {
        private static IKernel _kernel;

        private static void Main(string[] args)
        {
            //Setup log4net
            XmlConfigurator.Configure();

            //Setup Ninject
            SetupNinJect();

            var job = _kernel.Get<BackupDatabaseJob>() as IExecutableJob;
            job.ExecuteJob();
        }

        private static void SetupNinJect()
        {
            _kernel = new StandardKernel();
            _kernel.Bind<IConfigurationProvider>().To<ConfigurationProvider>();
            _kernel.Bind<IFileStorageService>().To<LocalDiskFileStorageService>();
            _kernel.Bind<IHttpClientService>().To<HttpClientService>();
            _kernel.Bind<IGearHostApiService>().To<GearHostApiService>();
            _kernel.Bind<ILog>().ToProvider<LogProvider>();
        }

        internal class LogProvider : Provider<ILog>
        {
            protected override ILog CreateInstance(IContext context)
            {
                var serviceName = context.Request.ParentRequest.Service;
                return LogManager.GetLogger(serviceName);
            }
        }
    }
}