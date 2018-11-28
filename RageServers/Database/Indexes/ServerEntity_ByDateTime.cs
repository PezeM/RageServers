using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RageServers.Entity;
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
