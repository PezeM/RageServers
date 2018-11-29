using RageServers.Entity;
using Raven.Client.Documents.Indexes;
using System.Linq;

namespace RageServers.Database.Indexes
{
    public class ServerEntity_ByServerInfo : AbstractIndexCreationTask<ServerEntity>
    {
        public ServerEntity_ByServerInfo()
        {
            Map = servers =>
                from server in servers
                select new
                {
                    server.ServerInfo
                };
        }
    }
}
