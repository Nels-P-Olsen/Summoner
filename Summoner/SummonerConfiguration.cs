using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using log4net.Core;
using RestSharp;
using Summoner.Clients;
using Summoner.Notifications;

namespace Summoner
{
    public class SummonerConfiguration
    {
        public SummonerConfiguration(string path)
        {
            string configString = System.IO.File.ReadAllText(path);
            dynamic configJson = SimpleJson.DeserializeObject(configString);

            PollInterval = (int)configJson["poll_interval"];
            ImageCacheDir = DefaultImageCacheDir;
            
            // TODO: Create actual ClientConfiguration and NotificationConfiguration objects (need ClientConfigurationFactory and NotificationConfigurationFactory)
            Clients = LoadClientConfigurations(LoadDynamic(configJson["clients"]));
            Notifications = LoadNotificationConfigurations(LoadDynamic(configJson["notifications"]));
        }

        /// <summary>
        /// Path for the image cache.
        /// </summary>
        public string ImageCacheDir
        {
            get;
            set;
        }

        /// <summary>
        /// The number of seconds to pause between polling clients
        /// </summary>
        public int PollInterval
        {
            get;
            set;
        }

        public IEnumerable<ClientConfiguration> Clients
        {
            get;
            set;
        }

        public IEnumerable<NotificationConfiguration> Notifications
        {
            get;
            set;
        }

        public static string DefaultConfigurationPath
        {
            get
            {
                string exe = System.Reflection.Assembly.GetEntryAssembly().Location;
                string configpath;

                if (exe.EndsWith(".exe", StringComparison.InvariantCultureIgnoreCase))
                {
                    configpath = exe.Substring(0, exe.Length - 4) + ".config";
                }
                else
                {
                    configpath = exe + ".config";
                }

                return configpath;
            }
        }

        private static string DefaultImageCacheDir
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    Path.Combine(Constants.ApplicationName, "Images"));
            }
        }

        private bool IsTrue(string value)
        {
            return (value.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("yes", StringComparison.OrdinalIgnoreCase));

        }

        private bool IsFalse(string value)
        {
            return (value.Equals("false", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("no", StringComparison.OrdinalIgnoreCase));

        }

        private List<ClientConfiguration> LoadClientConfigurations(
            IEnumerable<Dictionary<string, string>> values)
        {
            var clients = new List<ClientConfiguration>();

            foreach (var rawClientValues in values)
            {
                try
                {
                    clients.Add(ClientConfigurationFactory.NewClientConfiguration(this, rawClientValues));
                }
                catch (Exception ex)
                {
                    SummonerService.LogError(ex);
                }
            }

            return clients;
        }

        private List<NotificationConfiguration> LoadNotificationConfigurations(
            IEnumerable<Dictionary<string, string>> values)
        {
            var notifications = new List<NotificationConfiguration>();

            foreach (var rawNotificationValues in values)
            {
                try
                {
                    notifications.Add(NotificationConfigurationFactory.NewNotificationConfiguration(this, rawNotificationValues));
                }
                catch (Exception ex)
                {
                    SummonerService.LogError(ex);
                }
            }

            return notifications;
        }

        private IEnumerable<Dictionary<string, string>> LoadDynamic(dynamic input)
        {
            List<ConfigurationDictionary> result = new List<ConfigurationDictionary>();

            foreach (dynamic json in input)
            {
                ConfigurationDictionary o = new ConfigurationDictionary(this);

                foreach (KeyValuePair<string, dynamic> prop in json)
                {
                    o[prop.Key] = prop.Value.ToString();
                }

                result.Add(o);
            }

            return result;
        }
    }
}
