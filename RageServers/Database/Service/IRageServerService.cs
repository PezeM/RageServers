using System.Collections.Generic;
using System.Threading.Tasks;
using RageServers.Entity;

namespace RageServers.Database.Service
{
    public interface IRageServerService
    {
        Task InsertAsync(Dictionary<string, ServerInfo> servers);
        Task<ServerEntity> GetServerEntityAsync(string id);
        Task<IList<ServerEntity>> GetServerEntityByIpAsync(string ip);
    }
}