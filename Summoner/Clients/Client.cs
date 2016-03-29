using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summoner.Clients
{
    public interface Client
    {
        ClientConfiguration Configuration { get; }
        bool Running { get; }
        string Name { get; }
        void Start();
        IEnumerable<Message> RecentMessages();
        void Stop();
    }
}
