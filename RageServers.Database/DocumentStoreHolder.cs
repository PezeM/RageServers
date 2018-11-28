using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RageServers.Database.Indexes;
using RageServers.Models;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Operations;
using Raven.Client.Exceptions;
using Raven.Client.Exceptions.Database;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;

namespace RageServers.Database
{
    public class DocumentStoreHolder : IDocumentStoreHolder
    {
        public IDocumentStore Store { get; }
        private readonly ILogger<DocumentStoreHolder> _logger;

        public DocumentStoreHolder(IOptions<AppSettings> appSettings, ILogger<DocumentStoreHolder> logger)
        {
            _logger = logger;
            var ravenSettings = appSettings.Value.RavenSettings;

            Store = new DocumentStore()
            {
                Urls = new[] { ravenSettings.Url },
                Database = ravenSettings.Database
            };

            Store.Initialize();

            _logger.LogInformation($"Initialized RavenDB document store for {ravenSettings.Database} at {ravenSettings.Url}");

            // Create database if not exists
            CreateDatabaseIfNotExists();

            IndexCreation.CreateIndexes(typeof(ServerEntity_ByIP).Assembly, Store);
        }

        private void CreateDatabaseIfNotExists()
        {
            var database = Store.Database;

            if (string.IsNullOrWhiteSpace(database))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(database));

            try
            {
                Store.Maintenance.ForDatabase(database).Send(new GetStatisticsOperation());
            }
            catch (DatabaseDoesNotExistException)
            {
                try
                {
                    Store.Maintenance.Server.Send(new CreateDatabaseOperation(new DatabaseRecord(database)));
                }
                catch (ConcurrencyException)
                {
                    // The database was already created before calling CreateDatabaseOperation
                }

            }
        }
    }
}
