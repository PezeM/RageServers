using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RageServers.Models;

namespace RageServers.Database.Service
{
    public interface IRageServerService
    {
        Task InsertAsync(string ip, ServerInfo server);
        Task<bool> DeleteServerEntityAsync(string id);
        Task<ServerEntity> GetServerEntityAsync(string id);
        Task<List<ServerEntity>> GetServerEntitiesByIpAsync(string ip);
        Task<IEnumerable<ServerEntity>> GetAllServersAsync();
        int GetPeakPlayersForServer(string ip);
        int GetPeakPlayersForServerInDateRange(string ip, DateTime startDateTime, DateTime endDateTime);
        //int GetPeakPlayersForServerInDay(string ip, DateTime date);
        Task<Dictionary<DateTime, int>> GetPeakPlayersForServerForEachDayAsync(string ip);
    }
}