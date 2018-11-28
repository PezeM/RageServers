using RageServers.Entity;
using Raven.Client.Documents.Indexes;
using System.Linq;

namespace RageServers.Database.Indexes
{
    public class ServerEntity_ByIP : AbstractIndexCreationTask<ServerEntity>
    {
        public ServerEntity_ByIP()
        {
            Map = servers =>
                from server in servers
                select new
                {
                    server.IP
                };
        }
    }
}
