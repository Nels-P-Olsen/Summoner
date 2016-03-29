using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summoner.Notifications
{
    public abstract class NotificationConfiguration
    {
        public SummonerConfiguration Global { get; private set; }

        /// <summary>
        /// Limits notifications to messages that contain the given Regex pattern
        /// </summary>
        public string Contains { get; set; }

        public NotificationConfiguration(SummonerConfiguration global)
        {
            this.Global = global;
        }
    }
}
