using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RageServers.Database.Indexes;
using RageServers.Entity;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;

namespace RageServers.Database.Service
{
    public class RavenRageServerService : IRageServerService
    {
        private readonly IDocumentStore _store;

        public RavenRageServerService(IDocumentStoreHolder storeHolder)
        {
            _store = storeHolder.Store;
        }

        public async Task InsertAsync(string ip, ServerInfo server)
        {
            using (var session = _store.OpenAsyncSession())
            {
                var newServer = new ServerEntity
                {
                    Datetime = DateTime.Now,
                    IP = ip,
                    ServerInfo = server
                };

                await session.StoreAsync(newServer);
                await session.SaveChangesAsync();
            }
        }

        public async Task<ServerEntity> GetServerEntityAsync(string id)
        {
            using (var session = _store.OpenAsyncSession())
            {
                var server = await session.LoadAsync<ServerEntity>(id);
                return server;
            }
        }

        public async Task<bool> DeleteServerEntityAsync(string id)
        {
            using (var session = _store.OpenAsyncSession())
            {
                session.Delete(id);
                await session.SaveChangesAsync();
                return true;
            }
        }

        public async Task<IList<ServerEntity>> GetServerEntitiesByIpAsync(string ip)
        {
            using (var session = _store.OpenAsyncSession())
            {
                var servers = await session.Query<ServerEntity, ServerEntity_ByIP>()
                    .Where(s => s.IP == ip).ToListAsync();

                return servers;
            }
        }
    }
}
