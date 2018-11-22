using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LiteDB;
using RageServers.Entity;

namespace RageServers
{
    public class ServersDatabase
    {
        private string _connectionString;

        public ServersDatabase(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Insert(Dictionary<string, ServerInfo> servers)
        {
            using (var db = new LiteRepository(_connectionString))
            {
                foreach (var server in servers)
                {
                    db.Insert(new ServerEntity
                    {
                        IP = server.Key,
                        Datetime = DateTime.Now,
                        ServerInfo = server.Value
                    });
                }
            }
        }

        public IEnumerable<ServerEntity> GetServerInfo(string ip)
        {
            using (var db = new LiteRepository(_connectionString))
            {
                return db.Query<ServerEntity>().Where(q => q.IP == ip).ToEnumerable();
            }
        }

        public IEnumerable<ServerEntity> GetAll()
        {
            using (var db = new LiteRepository(_connectionString))
            {
                return db.Query<ServerEntity>().ToEnumerable();
            }
        }

        public int GetPeakPlayersForServer(string ip)
        {
            using (var db = new LiteRepository(_connectionString))
            {
                return db.Query<ServerEntity>().Where(q => q.IP == ip).ToEnumerable().Max(q => q.ServerInfo.Peak);
            }
        }

        public Dictionary<string, int> GetPeakForAllServers()
        {
            var timer = new Stopwatch();
            timer.Start();

            var allServers = GetAll();
            var peakDictionary = new Dictionary<string, int>();
            foreach (var serverEntity in allServers)
            {
                if (!peakDictionary.ContainsKey(serverEntity.IP))
                {
                    peakDictionary.Add(serverEntity.IP, GetPeakPlayersForServer(serverEntity.IP));
                }
            }

            timer.Stop();
            Console.WriteLine($"Time in ms {timer.ElapsedMilliseconds}, ticks {timer.ElapsedTicks}.");

            return peakDictionary;
        }
    }
}
