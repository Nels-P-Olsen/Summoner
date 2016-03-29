using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summoner.Notifications
{
    public class SmsNotificationConfiguration : NotificationConfiguration
    {
        /// <summary>
        /// User information for the Catapult API
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// API token for authentication
        /// </summary>
        public string ApiToken { get; set; }

        /// <summary>
        /// API secret for authentication
        /// </summary>
        public string ApiSecret { get; set; }

        /// <summary>
        /// sender phone number (in international format)
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// recipient phone number (in international format)
        /// </summary>
        public string To { get; set; }

        public SmsNotificationConfiguration(SummonerConfiguration global)
            : base(global)
        {
        }
    }
}
