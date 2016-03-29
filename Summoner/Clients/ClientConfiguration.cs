using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summoner.Clients
{
    public abstract class ClientConfiguration
    {
        public SummonerConfiguration Global { get; private set; }

        /// <summary>
        /// URI to the chat service
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        /// Type of authentication to use (basic or network)
        /// </summary>
        public string Authenticator { get; set; }

        /// <summary>
        /// Whether or not to use integrated security
        /// </summary>
        public bool IntegratedSecurity { get; set; }

        /// <summary>
        /// Username to authenticate with (if not using integrated security)
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password to authenticate with (if not using integrated security)
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// API token for authentication
        /// </summary>
        public string ApiToken { get; set; }

        /// <summary>
        /// Name of the room to monitor
        /// </summary>
        public string RoomName { get; set; }

        public ClientConfiguration(SummonerConfiguration global)
        {
            this.Global = global;
        }
    }
}
