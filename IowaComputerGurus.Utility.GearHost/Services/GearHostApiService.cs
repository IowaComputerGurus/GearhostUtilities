using System.Collections.Generic;
using System.Net;
using IowaComputerGurus.Utility.GearHost.Models;
using IowaComputerGurus.Utility.GearHost.Providers;

namespace IowaComputerGurus.Utility.GearHost.Services
{
    public interface IGearHostApiService
    {
        string ApiKey { get; }
        string BaseApiUrl { get; }
        GearHostGetDatabasesResponse GetDatabases();
        byte[] BackupDatabase(string databaseId);
    }

    public class GearHostApiService : IGearHostApiService
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IHttpClientService _httpClientService;

        public GearHostApiService(IConfigurationProvider configurationProvider, IHttpClientService httpClientService)
        {
            _configurationProvider = configurationProvider;
            _httpClientService = httpClientService;
        }

        public string ApiKey => _configurationProvider.AppSettings["GearHostApiKey"];
        public string BaseApiUrl => _configurationProvider.AppSettings["GearHostApiBaseUrl"];

        public GearHostGetDatabasesResponse GetDatabases()
        {
            //Make the get request
            var apiUrl = BaseApiUrl + "databases";
            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Authorization", $"bearer {ApiKey}")
            };
            return _httpClientService.MakeHttpGetRequest<GearHostGetDatabasesResponse>(apiUrl, string.Empty, headers);
        }

        public byte[] BackupDatabase(string databaseId)
        {
            //TODO: Jeremy review - Make better?
            //Setup the request
            var apiUrl = $"{BaseApiUrl}databases/{databaseId}/backup";
            var client = new WebClient();
            client.Headers.Add("Authorization", $"bearer {ApiKey}");
            return client.DownloadData(apiUrl);
        }
    }
}