using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Summoner.Clients;

namespace Summoner.Notifications
{
    public class ConsoleNotification : Notification
    {
        public ConsoleNotification(ConsoleNotificationConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        NotificationConfiguration Notification.Configuration
        {
            get { return this.Configuration; }    
        }

        public ConsoleNotificationConfiguration Configuration
        {
            get;
            private set;
        }

        public void Notify(Client client, Message message)
        {
            Console.WriteLine(message.Time + " / " + client.Name + " / " + message.Sender + " / " + message.Content);
        }
    }
}
