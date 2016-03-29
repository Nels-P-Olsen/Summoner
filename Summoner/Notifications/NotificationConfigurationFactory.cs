using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summoner.Notifications
{
    public class NotificationConfigurationFactory
    {
        public static NotificationConfiguration NewNotificationConfiguration(SummonerConfiguration global, IDictionary<string, string> rawValues)
        {
            if (!rawValues.ContainsKey("type") || string.IsNullOrWhiteSpace(rawValues["type"]))
                throw new ArgumentOutOfRangeException("notification", "notification type not specified");


            switch (rawValues["type"])
            {
                case "console":
                    return new ConsoleNotificationConfiguration(global)
                    {
                        Contains = GetValueOrDefault<string>(rawValues, "contains"),
                    };

                case "growl":
                    return new GrowlNotificationConfiguration(global)
                    {
                        Contains = GetValueOrDefault<string>(rawValues, "contains"),
                    };

                case "metro":
                    return new MetroNotificationConfiguration(global)
                    {
                        Contains = GetValueOrDefault<string>(rawValues, "contains"),
                        DebugInstall =  GetValueOrDefault<bool>(rawValues, "debug-install", false),
                        Audio = GetValueOrDefault<bool>(rawValues, "audio", true),
                    };

                case "sms":
                    return new SmsNotificationConfiguration(global)
                    {
                        Contains = GetValueOrDefault<string>(rawValues, "contains"),
                        UserId = GetValueOrDefault<string>(rawValues, "userid"),
                        ApiToken = GetValueOrDefault<string>(rawValues, "api-token"),
                        ApiSecret = GetValueOrDefault<string>(rawValues, "api-secret"),
                        From = GetValueOrDefault<string>(rawValues, "from"),
                        To = GetValueOrDefault<string>(rawValues, "to"),
                    };

                default:
                    throw new ArgumentOutOfRangeException("notification.type", "unrecognized notification type: " + rawValues["type"]);

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
