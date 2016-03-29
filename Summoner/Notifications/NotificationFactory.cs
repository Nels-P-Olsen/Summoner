using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Summoner.Util;

namespace Summoner.Notifications
{
    public class NotificationFactory
    {
        public static Notification NewNotification(NotificationConfiguration config)
        {
            Assert.NotNull(config, "config");

            var consoleConfig = config as ConsoleNotificationConfiguration;
            if (consoleConfig != null)
            {
                return new ConsoleNotification(consoleConfig);
            }

            var growlConfig = config as GrowlNotificationConfiguration;
            if (growlConfig != null)
            {
                return new GrowlNotification((GrowlNotificationConfiguration)config);
            }

            var smsConfig = config as SmsNotificationConfiguration;
            if (smsConfig != null)
            {
                return new SmsNotification(smsConfig);
            }

            var metroConfig = config as MetroNotificationConfiguration;
            if (metroConfig != null)
            {
                return new MetroNotification(metroConfig);
            }

            throw new UnknownNotificationTypeException(
                "Unknown notification type: " + config.GetType().Name);
        }
    }

    public class UnknownNotificationTypeException : Exception
    {
        public UnknownNotificationTypeException(string message)
            : base(message)
        {
        }
    }
}
