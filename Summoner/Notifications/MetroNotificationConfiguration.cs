using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summoner.Notifications
{
    public class MetroNotificationConfiguration : NotificationConfiguration
    {
        public bool DebugInstall
        {
            get;
            set;
        }

        public bool Audio
        {
            get;
            set;
        }

        public MetroNotificationConfiguration(SummonerConfiguration global)
            : base(global)
        {
        }
    }
}
