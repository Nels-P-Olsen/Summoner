using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Summoner.Rest;
using Summoner.Rest.Campfire;
using Summoner.Util;

namespace Summoner.Clients
{
    public class CampfireClient : Client
    {
        private readonly SummonerRestClientConfiguration restConfiguration = new SummonerRestClientConfiguration();
        private readonly string roomName;

        private readonly ImageManager imageManager;

        private readonly Object runningLock = new Object();
        private bool running;

        private CampfireRestClient restClient;
        private CampfireRoom room;
        private int? hwm = null;

        private readonly Dictionary<int, CampfireUser> users = new Dictionary<int, CampfireUser>();

        public CampfireClient(CampfireClientConfiguration configuration)
        {
            this.Configuration = configuration;

            restConfiguration.Uri = configuration.Uri;
            restConfiguration.Username = configuration.ApiToken;
            restConfiguration.Password = "X";
            roomName = configuration.RoomName;

            imageManager = new ImageManager(this);
        }

        ClientConfiguration Client.Configuration
        {
            get { return this.Configuration; }
        }

        public ClientConfiguration Configuration
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

        public string Name
        {
            get { return roomName ?? "Campfire Rooms"; }
        }

        public void Start()
        {
            lock (runningLock)
            {
                if (running)
                {
                    Stop();
                }

                restClient = new CampfireRestClient(restConfiguration);
                room = null;

                foreach (CampfireRoom r in restClient.Rooms())
                {
                    if (roomName.Equals(r.Name))
                    {
                        room = r;
                        break;
                    }
                }

                if (room == null)
                {
                    throw new Exception(String.Format("Could not find room '{0}'", roomName));
                }

                CampfireMessage lastMessage = restClient.Messages(room).Last();

                if (lastMessage != null)
                {
                    hwm = lastMessage.Id;
                }

                running = true;
            }

            new Thread(delegate()
            {
                imageManager.Start();
            }).Start();
        }

        private CampfireUser GetUser(int userId)
        {
            lock (users)
            {
                if (!users.ContainsKey(userId))
                {
                    CampfireUser user = restClient.User(userId);

                    if (user != null)
                    {
                        users[userId] = user;
                    }
                    else
                    {
                        users[userId] = new CampfireUser();
                        users[userId].Name = "Unknown user " + userId.ToString();
                    }
                }

                return users[userId];
            }
        }

        private string ResolveUserId(int userId)
        {
            return GetUser(userId).Name;
        }

        private string ResolveUserImage(int userId)
        {
            string email = GetUser(userId).EmailAddress;

            if (email == null)
            {
                return null;
            }

            byte[] emailHash = new MD5CryptoServiceProvider().ComputeHash(
                Encoding.UTF8.GetBytes(email.Trim().ToLower()));

            string imageUrl = String.Format("http://www.gravatar.com/avatar/{0}",
                StringUtil.HexString(emailHash));

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

                CampfireRestClient.CampfireMessageOptions messageOptions =
                    (hwm != null) ?
                    new CampfireRestClient.CampfireMessageOptions() { SinceMessageId = hwm } :
                    null;

                IEnumerable<CampfireMessage> campfireMessages = restClient.Messages(room, messageOptions);
                List<Message> messages = new List<Message>();

                foreach (CampfireMessage campfireMessage in campfireMessages)
                {
                    hwm = campfireMessage.Id;

                    if (!campfireMessage.Type.Equals("TextMessage"))
                    {
                        continue;
                    }

                    int userId = campfireMessage.UserId;

                    messages.Add(new Message()
                    {
                        Sender = ResolveUserId(campfireMessage.UserId),
                        SenderImage = ResolveUserImage(campfireMessage.UserId),
                        Time = campfireMessage.CreatedAt,
                        Content = campfireMessage.Body
                    });
                }

                return messages;
            }
        }

        public void Stop()
        {
            lock (runningLock)
            {
                restClient = null;
                room = null;

                running = false;
            }

            imageManager.Stop();
        }
    }
}
