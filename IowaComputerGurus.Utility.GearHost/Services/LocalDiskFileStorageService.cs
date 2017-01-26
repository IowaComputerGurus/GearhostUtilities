using System;
using System.IO;
using System.Linq;
using IowaComputerGurus.Utility.GearHost.Providers;
using log4net;

namespace IowaComputerGurus.Utility.GearHost.Services
{
    public interface IFileStorageService
    {
        string RootFolder { get; }
        void StoreDatabaseFile(string databaseName, byte[] backupBytes);
    }

    public class LocalDiskFileStorageService : IFileStorageService
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly ILog _log;

        public string RootFolder => _configurationProvider.AppSettings["LocalFileStorageService-RootPath"];
        public int MaxVersions => int.Parse(_configurationProvider.AppSettings["LocalFileStorageService-MaxVersions"]);

        public LocalDiskFileStorageService(IConfigurationProvider configurationProvider, ILog log)
        {
            _configurationProvider = configurationProvider;
            _log = log;
        }

        public void StoreDatabaseFile(string databaseName, byte[] backupBytes)
        {
            _log.Info($"Storing backup for '{databaseName}' database");
            //Setup paths & names
            var currentDate = DateTime.Now;
            var destinationDirectory = Path.Combine(RootFolder, databaseName);
            _log.Debug($"Destination Directory: {destinationDirectory}");
            var fileName = $"{currentDate.Year}{currentDate.Month}{currentDate.Day}-{databaseName}.zip";
            _log.Debug($"Target File: {fileName}");
            var fullPath = Path.Combine(destinationDirectory, fileName);

            //Verify the destination exists
            if (!Directory.Exists(destinationDirectory))
            {
                _log.Info($"Creating storage directory '{destinationDirectory}");
                Directory.CreateDirectory(destinationDirectory);
            }

            //Write the full path
            File.WriteAllBytes(fullPath, backupBytes);

            //History
            CleanOldVersions(destinationDirectory);
        }

        private void CleanOldVersions(string destinationDirectory)
        {
            //Verify version count
            var directoryInf = new DirectoryInfo(destinationDirectory);
            var files = directoryInf.GetFiles("*.zip").OrderByDescending(f => f.CreationTime);
            if (files.Count() < MaxVersions)
                return;

            //Remove old
            _log.Info($"Cleaning up older backups beyond the {MaxVersions} limit");
            var toRemove = files.Skip(MaxVersions).ToList();
            foreach (var file in toRemove)
            {
                try
                {
                    _log.Info($"Deleting: {file.Name}");
                    file.Delete();
                }
                catch (Exception ex)
                {
                    _log.Error("Delete file error", ex);
                }
            }
        }
    }
}