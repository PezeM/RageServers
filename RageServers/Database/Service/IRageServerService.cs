using System.Collections.Generic;
using System.Threading.Tasks;
using RageServers.Entity;

namespace RageServers.Database.Service
{
    public interface IRageServerService
    {
        Task InsertAsync(string ip, ServerInfo server);
        Task<bool> DeleteServerEntityAsync(string id);
        Task<ServerEntity> GetServerEntityAsync(string id);
        Task<IList<ServerEntity>> GetServerEntitiesByIpAsync(string ip);
    }
}