using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Growl;
using Growl.Connector;
using Growl.CoreLibrary;
using Summoner.Clients;

namespace Summoner.Notifications
{
    public class GrowlNotification : Notification
    {
        private const string NotificationName = "MESSAGE";

        private readonly GrowlConnector growlConnector;

        public GrowlNotification(GrowlNotificationConfiguration configuration)
        {
            this.Configuration = configuration;

            Application growlApplication = new Application(Constants.ApplicationName);

            growlConnector = new GrowlConnector();
            growlConnector.Register(growlApplication, new NotificationType[]
            {
                new NotificationType(NotificationName, "Incoming Message")
            });
        }

        NotificationConfiguration Notification.Configuration
        {
            get { return this.Configuration; }
        }

        public GrowlNotificationConfiguration Configuration
        {
            get;
            private set;
        }

        public void Notify(Client client, Message message)
        {
            growlConnector.Notify(new Growl.Connector.Notification(
                applicationName: Constants.ApplicationName,
                notificationName: NotificationName,
                id: "",
                title: client.Name,
                text: String.Format("[{0}] {1}", message.Sender, message.Content),
                icon: GetImageIfPossible(message.SenderImage),
                sticky: message.IsSticky,
                priority: message.IsSticky ? Priority.High : Priority.Normal,
                coalescingID: ""));
        }

        private Image GetImageIfPossible(string path)
        {
            if (String.IsNullOrEmpty(path)) return null;
            if (!System.IO.File.Exists(path)) return null;

            try
            {
                return Growl.CoreLibrary.ImageConverter.ImageFromBytes(System.IO.File.ReadAllBytes(path));
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
