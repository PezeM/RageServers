using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Get ServerEntity from database by id
        /// </summary>
        /// <param name="id">ID of the entity</param>
        /// <returns><see cref="ServerEntity"/></returns>
        public async Task<ServerEntity> GetServerEntityAsync(string id)
        {
            using (var session = _store.OpenAsyncSession())
            {
                var server = await session.LoadAsync<ServerEntity>(id);
                return server;
            }
        }

        /// <summary>
        /// Deletes server entity from databse by id
        /// </summary>
        /// <param name="id">ID of the entity to remove</param>
        public async Task<bool> DeleteServerEntityAsync(string id)
        {
            using (var session = _store.OpenAsyncSession())
            {
                session.Delete(id);
                await session.SaveChangesAsync();
                return true;
            }
        }

        /// <summary>
        /// Gets all server entities from database by ip
        /// </summary>
        /// <param name="ip">IP of the Rage server</param>
        /// <returns>List of <see cref="ServerEntity"/></returns>
        public async Task<List<ServerEntity>> GetServerEntitiesByIpAsync(string ip)
        {
            using (var session = _store.OpenAsyncSession())
            {
                return await session.Query<ServerEntity, ServerEntity_ByIP>()
                    .Where(s => s.IP == ip)
                    .ToListAsync();
            }
        }

        /// <summary>
        /// Get all server entities from database
        /// </summary>
        /// <returns><see cref="IEnumerable{ServerEntity}"/> of <see cref="ServerEntity"/></returns>
        public async Task<IEnumerable<ServerEntity>> GetAllServersAsync()
        {
            using (var session = _store.OpenAsyncSession())
            {
                return await session.Advanced.AsyncDocumentQuery<ServerEntity, ServerEntity_ByIP>()
                    .ToListAsync();
            }
        }

        /// <summary>
        /// Get highest number of players for server from database by ip
        /// </summary>
        /// <param name="ip">IP of Rage server</param>
        /// <returns>Highest number of players</returns>
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

        /// <summary>
        /// Get highest number of players for server from database by ip in date range
        /// </summary>
        /// <param name="ip">IP of Rage server</param>
        /// <param name="startDateTime">Start date</param>
        /// <param name="endDateTime">End date</param>
        /// <returns>Highest number of players</returns>
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
                return list.GroupBy(q => q.Datetime.Date).ToDictionary(q => q.Key,
                    q => q.Max(x => x.ServerInfo.Players));
            }
        }
    }
}
