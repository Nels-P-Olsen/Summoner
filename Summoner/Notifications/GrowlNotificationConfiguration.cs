using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summoner.Notifications
{
    public class GrowlNotificationConfiguration : NotificationConfiguration
    {
        public GrowlNotificationConfiguration(SummonerConfiguration global)
            : base(global)
        {
        }
    }
}
