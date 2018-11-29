using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RageServers.Database.Indexes;
using RageServers.Entity;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Operations;

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
                var datetime = DateTime.Now;
                var newServer = new ServerEntity
                {
                    Datetime = datetime.AddTicks(-(datetime.Ticks % TimeSpan.TicksPerSecond)), // Removes ms from datetime
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
                    .Where(s => s.IP == ip)
                    .ToListAsync();

                return servers;
            }
        }

        public async Task<IEnumerable<ServerEntity>> GetAllServersAsync()
        {
            using (var session = _store.OpenAsyncSession())
            {
                return await session.Advanced.AsyncDocumentQuery<ServerEntity, ServerEntity_ByIP>().ToListAsync();
            }
        }

        public int GetPeakPlayersForServer(string ip)
        {
            using (var session = _store.OpenSession())
            {
                return session.Query<ServerEntity>().Where(q => q.IP == ip)
                    .ToList()
                    .Max(q => q.ServerInfo.Peak);

                //return session.Query<ServerEntity>().Where(q => q.IP == ip).OrderBy(q => q.ServerInfo.Peak).Take(1)
                //    .Select(q => q.ServerInfo.Peak).FirstOrDefault();
            }
        }

        public int GetPeakPlayersForServerInDateRange(string ip, DateTime startDateTime, DateTime endDateTime)
        {
            using (var session = _store.OpenSession())
            {
                return session.Advanced.DocumentQuery<ServerEntity>()
                    .WhereEquals(q => q.IP, ip)
                    .WhereBetween(q => q.Datetime, startDateTime, endDateTime)
                    .OrderByDescending(q => q.ServerInfo.Peak)
                    .ToList()
                    .Select(q => q.ServerInfo.Peak)
                    .FirstOrDefault();
            }
        }
    }
}
