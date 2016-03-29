using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Summoner.Rest;
using Summoner.Rest.Tfs;
using Summoner.Util;

namespace Summoner.Clients
{
    public class TfsClient : Client
    {
        private readonly SummonerRestClientConfiguration restConfiguration = new SummonerRestClientConfiguration();
        private readonly string roomName;
		private string myTfsUsername;

		private readonly ImageManager imageManager;

        private readonly Object runningLock = new Object();
        private bool running;

        private TfsTeamRoomRestClient restClient;
        private TfsTeamRoom room;
        private DateTime lastMessageTime = DateTime.UtcNow;
        private int hwm;

        private readonly Dictionary<Guid, string> userNames = new Dictionary<Guid, string>();

        public TfsClient(TfsClientConfiguration configuration)
        {
            this.Configuration = configuration;

            restConfiguration.Uri = configuration.Uri;
            restConfiguration.Authenticator = configuration.Authenticator;
            restConfiguration.Username = configuration.Username;
            restConfiguration.Password = configuration.Password;
            restConfiguration.IntegratedSecurity = configuration.IntegratedSecurity;
            roomName = configuration.RoomName;

            imageManager = new ImageManager(this);
        }

        public TfsClientConfiguration Configuration
        {
            get;
            private set;
        }

        ClientConfiguration Client.Configuration
        {
            get { return this.Configuration; }
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

        public string Name
        {
            get { return this.roomName ?? "TFS Rooms"; }
        }

        public void Start()
        {
            lock (runningLock)
            {
                if (running)
                {
                    Stop();
                }

                TfsUserProfileRestClient profileClient = new TfsUserProfileRestClient(restConfiguration);
                myTfsUsername = profileClient.GetIdentity().DisplayName;

                restClient = new TfsTeamRoomRestClient(restConfiguration);

                room = null;

                foreach (TfsTeamRoom r in restClient.Rooms())
                {
                    if (roomName.Equals(r.Name))
                    {
                        room = r;
                        break;
                    }
                }

                if (room == null)
                {
                    throw new Exception(String.Format("Could not find team room '{0}'", roomName));
                }

                /* Load the high-water mark */
                IEnumerable<TfsTeamRoomMessage> messages = restClient.Messages(room);

                TfsTeamRoomMessage lastMessage = (messages.Count() > 0) ? restClient.Messages(room).Last() : null;

                lastMessageTime = (lastMessage != null) ? lastMessage.PostedTime : DateTime.UtcNow;
                hwm = (lastMessage != null) ? lastMessage.Id : 0;

                running = true;
            }

            new Thread(delegate()
            {
                imageManager.Start();
            }).Start();
        }

        private string ResolveUserId(Guid userTfId)
        {
            lock (userNames)
            {
                if (!userNames.ContainsKey(userTfId))
                {
                    TfsUserProfileRestClient profileClient = new TfsUserProfileRestClient(restConfiguration);
                    TfsUserIdentity identity = profileClient.GetIdentity(userTfId.ToString());

                    if (identity != null)
                    {
                        userNames[userTfId] = identity.DisplayName;
                    }
                    else
                    {
                        userNames[userTfId] = "Unknown user " + userTfId.ToString();
                    }
                }

                return userNames[userTfId];
            }
        }

        private string ResolveUserImage(Guid userTfId)
        {
            string imageUrl = String.Format("{0}/_api/_common/IdentityImage?id={1}&__v=5",
                Configuration.Uri.AbsoluteUri, userTfId.ToString());

            return imageManager.GetImage(imageUrl);
        }

        public IEnumerable<Message> RecentMessages()
        {
            lock (runningLock)
            {
                if (!running)
                {
                    throw new Exception("Not connected");
                }

                string lastMessageTimeFilter = "PostedTime gt " + lastMessageTime.ToUniversalTime().ToString(GetRequestTimeFormat());
                IEnumerable<TfsTeamRoomMessage> teamMessages = restClient.Messages(room, lastMessageTimeFilter);

                List<Message> messages = new List<Message>();
                foreach (TfsTeamRoomMessage teamMessage in teamMessages)
                {
                    lastMessageTime = teamMessage.PostedTime;

                    if (teamMessage.Id <= hwm)
                    {
                        continue;
                    }

                    hwm = teamMessage.Id;

                    messages.Add(new Message()
                    {
                        Sender = ResolveUserId(teamMessage.PostedByUserTfid),
                        SenderImage = ResolveUserImage(teamMessage.PostedByUserTfid),
                        Time = teamMessage.PostedTime,
                        Content = teamMessage.Content,
                        IsSticky = IsSticky(teamMessage)
                    });
                }

                return messages;
            }
        }

        private bool IsSticky(TfsTeamRoomMessage teamMessage)
        {
            return teamMessage.Content.Contains("@" + myTfsUsername);
        }

        private string GetRequestTimeFormat()
        {
            switch (Configuration.Authenticator)
            {
                case "network": // TFS On-premises
                    return RestSharp.DateFormat.ROUND_TRIP;

                case "basic": // Visual Studio Online
                default:
                    return RestSharp.DateFormat.ISO_8601;
            }   
        }

        public void Stop()
        {
            lock (runningLock)
            {
                restClient = null;
                room = null;
                lastMessageTime = default(DateTime);
                hwm = 0;

                running = false;
            }

            imageManager.Stop();
        }
    }
}
