using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Xsl;

namespace Summoner.Clients
{
    public class ClientConfigurationFactory
    {
        public static ClientConfiguration NewClientConfiguration(SummonerConfiguration global, IDictionary<string, string> rawValues)
        {
            if (!rawValues.ContainsKey("type") || string.IsNullOrWhiteSpace(rawValues["type"]))
                throw new ArgumentOutOfRangeException("client", "client type not specified");

            switch (rawValues["type"])
            {
                case "campfire":
                    return new CampfireClientConfiguration(global)
                    {
                        Uri = new Uri(GetValueOrDefault<string>(rawValues, "uri")),
                        RoomName = GetValueOrDefault<string>(rawValues, "room"),
                        ApiToken = GetValueOrDefault<string>(rawValues, "api-token")
                    };

                case "tfs":
                    return new TfsClientConfiguration(global)
                    {
                        Uri = new Uri(GetValueOrDefault<string>(rawValues, "uri")),
                        RoomName = GetValueOrDefault<string>(rawValues, "room"),
                        Authenticator = GetValueOrDefault<string>(rawValues, "authenticator", "basic"),
                        IntegratedSecurity = GetValueOrDefault<bool>(rawValues, "integrated-security"),
                        Username = GetValueOrDefault<string>(rawValues, "username"),
                        Password = GetValueOrDefault<string>(rawValues, "password"),
                    };

                default:
                    throw new ArgumentOutOfRangeException("client.type", "unrecognized client type: " + rawValues["type"]);

            }
        }

        private static T GetValueOrDefault<T>(IDictionary<string, string> dict, string key)
        {
            return GetValueOrDefault<T>(dict, key, default(T));
        }

        private static T GetValueOrDefault<T>(IDictionary<string, string> dict, string key, T defaultValue)
        {
            return (T)(dict.ContainsKey(key) ? Convert.ChangeType(dict[key], typeof(T)) : defaultValue);
        }
    }
}
