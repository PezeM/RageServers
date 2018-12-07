using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RageServers.Database.Indexes;
using RageServers.Models;
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

        /// <summary>
        /// Insert server information to database
        /// </summary>
        /// <param name="ip">Server IP</param>
        /// <param name="server">Server information</param>
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

        public async Task<List<ServerEntity>> GetServerEntitiesByIpAsync(string ip)
        {
            using (var session = _store.OpenAsyncSession())
            {
                return await session.Query<ServerEntity, ServerEntity_ByIP>()
                    .Where(s => s.IP == ip)
                    .ToListAsync();
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

        /// <summary>
        /// Get <see cref="Dictionary{TKey,TValue}"/> with maxium number of players on the server for each day.
        /// </summary>
        /// <param name="ip">Ip of the server</param>
        /// <returns>Dictionary with DateTime and number of players</returns>
        public async Task<Dictionary<DateTime, int>> GetPeakPlayersForServerForEachDayAsync(string ip)
        {
            using (var session = _store.OpenSession())
            {
                var list = await GetServerEntitiesByIpAsync(ip);
                return list.GroupBy(q => q.Datetime.Date)
                    .ToDictionary
                    (q => q.Key,
                    q => q.Max(x => x.ServerInfo.Players));
            }
        }
    }
}
