using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Summoner.Rest;
using Summoner.Rest.Bandwidth;
using Summoner.Clients;

namespace Summoner.Notifications
{
    public class SmsNotification : Notification
    {
        private BandwidthRestClient restClient;

        public SmsNotification(SmsNotificationConfiguration configuration)
        {
            this.Configuration = configuration;

            SummonerRestClientConfiguration restClientConfig = new SummonerRestClientConfiguration();
            restClientConfig.Uri = new Uri("https://api.catapult.inetwork.com/");
            restClientConfig.Username = Configuration.ApiToken;
            restClientConfig.Password = Configuration.ApiSecret;

            restClient = new BandwidthRestClient(restClientConfig);
        }

        NotificationConfiguration Notification.Configuration
        {
            get { return this.Configuration; }
        }

        public SmsNotificationConfiguration Configuration
        {
            get;
            private set;
        }

        public void Notify(Client client, Message message)
        {
            string notification = String.Format("[{0}:{1}] {2}",
                client.Name, message.Sender, message.Content);
            restClient.SendMessage(Configuration.UserId, Configuration.From, Configuration.To, notification);
        }
    }
}
