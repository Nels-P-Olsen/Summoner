using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Summoner.Util;

namespace Summoner.Clients
{
    public class ClientFactory
    {
        public static Client NewClient(ClientConfiguration configuration)
        {
            Assert.NotNull(configuration, "configuration");

            var tfsClientConfiguration = configuration as TfsClientConfiguration;
            if (tfsClientConfiguration != null)
            {
                return new TfsClient(tfsClientConfiguration);
            }

            var campfireClientConfiguration = configuration as CampfireClientConfiguration;
            if (campfireClientConfiguration != null)
            {
                return new CampfireClient(campfireClientConfiguration);
            }

            throw new UnknownClientTypeException("Unknown client type: " + configuration.GetType().Name);
        }
    }

    public class UnknownClientTypeException : Exception
    {
        public UnknownClientTypeException(string message)
            : base(message)
        {
        }
    }
}
