using System.Collections.Specialized;
using System.Configuration;

namespace IowaComputerGurus.Utility.GearHost.Providers
{
    public interface IConfigurationProvider
    {
        NameValueCollection AppSettings { get; }
    }

    public class ConfigurationProvider : IConfigurationProvider
    {
        public NameValueCollection AppSettings => ConfigurationManager.AppSettings;
    }
}