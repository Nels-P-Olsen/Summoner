using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summoner.Notifications
{
    public class ConsoleNotificationConfiguration : NotificationConfiguration
    {
        public ConsoleNotificationConfiguration(SummonerConfiguration global)
            : base(global)
        {
        }
    }
}
