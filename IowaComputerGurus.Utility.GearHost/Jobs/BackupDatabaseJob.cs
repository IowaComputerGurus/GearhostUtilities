using System.Linq;
using IowaComputerGurus.Utility.GearHost.Providers;
using IowaComputerGurus.Utility.GearHost.Services;
using log4net;

namespace IowaComputerGurus.Utility.GearHost.Jobs
{
    public class BackupDatabaseJob : IExecutableJob
    {
        private readonly IGearHostApiService _gearHostApiService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly ILog _log;

        public string SkipDatabases => _configurationProvider.AppSettings["BackupDatabaseJob-SkipDatabases"];

        public BackupDatabaseJob(IFileStorageService fileStorageService, IGearHostApiService gearHostApiService, IConfigurationProvider configurationProvider, ILog log)
        {
            _fileStorageService = fileStorageService;
            _gearHostApiService = gearHostApiService;
            _configurationProvider = configurationProvider;
            _log = log;
        }

        public void ExecuteJob()
        {
            //Split out the skip databases
            _log.Info("Starting backup process from GearHost");
            var skipDbList = SkipDatabases.Split(',');

            //Get the databases
            _log.Info("Getting database list");
            var allDatabases = _gearHostApiService.GetDatabases();
            _log.Info($"Found {allDatabases.Databases.Count} databases");
            foreach (var item in allDatabases.Databases)
            {
                if (skipDbList.Contains(item.Name))
                {
                    _log.Debug($"Skipping {item.Name} due to configuration");
                    continue;
                }

                _log.Info($"Requesting backup for database '{item.Name}' size {item.Size}");
                var data = _gearHostApiService.BackupDatabase(item.Id);

                _fileStorageService.StoreDatabaseFile(item.Name, data);
            }
        }
    }
}