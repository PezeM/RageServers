using System.Linq;
using RageServers.Models;
using Raven.Client.Documents.Indexes;

namespace RageServers.Database.Indexes
{
    public class ServerEntity_ByDateTime : AbstractIndexCreationTask<ServerEntity>
    {
        public ServerEntity_ByDateTime()
        {
            Map = servers =>
                from server in servers
                select new
                {
                    server.Datetime
                };
        }
    }
}
