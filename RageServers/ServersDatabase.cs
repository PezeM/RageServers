using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LiteDB;
using RageServers.Entity;
using RageServers.Exceptions;

namespace RageServers
{
    public class ServersDatabase
    {
        private string _connectionString;

        public ServersDatabase(string connectionString)
        {
            _connectionString = connectionString;
            InitializeDatabase();

        }

        private void InitializeDatabase()
        {
            // Throws exeption if there is no Filename argument in the connection string
            if (!_connectionString.Contains("Filename="))
            {
                throw new WrongDatabasePathException(_connectionString);
            }

            var databasePath = _connectionString.Remove(_connectionString.IndexOf("Filename="), "Filename=".Length);
            // Create new database with indexes if there is none
            if (!File.Exists(databasePath))
            {
                CreateIndexes();
            }
        }

        /// <summary>
        /// Method for ensuring indexes at startup
        /// </summary>
        private void CreateIndexes()
        {
            using (var db = new LiteDatabase(_connectionString))
            {
                var collection = db.GetCollection<ServerEntity>();
                collection.EnsureIndex(x => x.IP);
                collection.EnsureIndex(x => x.Datetime);
            }
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

        public void Delete(ServerEntity server)
        {
            using (var db = new LiteRepository(_connectionString))
            {
                db.Delete<ServerEntity>(server.Id);
            }
        }

        public void Delete(Dictionary<string, ServerEntity> servers)
        {
            using (var db = new LiteRepository(_connectionString))
            {
                foreach (var server in servers)
                {
                    db.Delete<Dictionary<string, ServerEntity>>(server.Value.Id);
                }
            }
        }

        public void DeleteMultiple(IEnumerable<ServerEntity> servers)
        {
            using (var db = new LiteRepository(_connectionString))
            {
                foreach (var server in servers)
                {
                    db.Delete<ServerEntity>(server.Id);
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

        public int GetPeakPlayersForServerInDateRange(string ip, DateTime startTime, DateTime endTime)
        {
            using (var db = new LiteRepository(_connectionString))
            {
                try
                {
                    return db.Query<ServerEntity>().Where(q => q.IP == ip && q.Datetime > startTime && q.Datetime < endTime)
                        .ToEnumerable().Max(q => q.ServerInfo.Peak);
                }
                catch (InvalidOperationException e)
                {
                    Console.WriteLine($"No records found in given timerange! \n {e}");
                    return 0;
                }
            }
        }

        public Dictionary<string, int> GetPeakForAllServers()
        {
            var allServers = GetAll();
            var peakDictionary = new Dictionary<string, int>();
            foreach (var serverEntity in allServers)
            {
                if (!peakDictionary.ContainsKey(serverEntity.IP))
                {
                    peakDictionary.Add(serverEntity.IP, GetPeakPlayersForServer(serverEntity.IP));
                }
            }

            return peakDictionary;
        }
    }


}
