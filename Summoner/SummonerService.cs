using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Summoner;
using Summoner.Clients;
using Summoner.Notifications;
using Summoner.Util;

namespace Summoner
{
    public class SummonerService
    {
        private Object runningLock = new Object();
        private bool running;
        private DateTime? startTime;

        public SummonerService(SummonerConfiguration config)
        {
            Config = config;
        }

        public SummonerConfiguration Config
        {
            get;
            private set;
        }

        public bool Running
        {
            get
            {
                lock (runningLock)
                {
                    return running;
                }
            }
        }

        public DateTime? StartTime
        {
            get
            {
                lock (runningLock)
                {
                    return startTime;
                }
            }
        }

        public void Start()
        {
            lock (runningLock)
            {
                if (Running) return;
            }

            List<Client> clients = new List<Client>();
            foreach (ClientConfiguration clientConfig in Config.Clients)
            {
                try
                {
                    Client client = ClientFactory.NewClient(clientConfig);
                    client.Start();
                    clients.Add(client);
                }
                catch (UnknownClientTypeException e)
                {
                    LogError(e.Message);
                }
            }

            List<Notification> notifications = new List<Notification>();
            foreach (NotificationConfiguration notificationConfig in Config.Notifications)
            {
                try
                {
                    notifications.Add(NotificationFactory.NewNotification(notificationConfig));
                }
                catch (UnknownNotificationTypeException e)
                {
                    LogError(e.Message);
                }
            }

            lock (runningLock)
            {
                this.running = true;
                this.startTime = DateTime.Now;
            }

            while (Running)
            {
                foreach (Client client in clients)
                {
                    IEnumerable<Message> messages;

                    try
                    {
                        messages = client.RecentMessages();
                    }
                    catch (Exception e)
                    {
                        LogError("Could not receive messages from {0}: {1} (ignoring)",
                            client.GetType().Name, e.Message);
                        continue;
                    }

                    foreach (Message message in messages)
                    {
                        foreach (Notification notification in notifications)
                        {
                            if (notification.Configuration == null) continue;
                            
                            if (!string.IsNullOrEmpty(notification.Configuration.Contains) &&
                                !message.Content.Contains(notification.Configuration.Contains))
                            {
                                continue;
                            }

                            try
                            {
                                notification.Notify(client, message);
                            }
                            catch (Exception e)
                            {
                                LogError("Could not notify using {0}: {1} (ignoring)",
                                    notification.Configuration.GetType().Name, e.Message);
                                continue;
                            }
                        }
                    }
                }

                Thread.Sleep(Config.PollInterval * 1000);
            }

            foreach (Client client in clients)
            {
                client.Stop();
            }

            lock (runningLock)
            {
                running = false;
                startTime = null;
            }
        }
        public void Stop()
        {
            lock (runningLock)
            {
                running = false;
            }
        }

        public static void LogError(Exception ex)
        {
            // TODO: log with log4net
        }

        public static void LogError(string format, params object[] args)
        {
            // TODO: log with log4net
        }
    }
}
