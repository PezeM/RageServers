using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RageServers.Models;
using Raven.Client.Documents;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;

namespace RageServers.Database
{
    public class DocumentStoreHolder : IDocumentStoreHolder
    {
        public IDocumentStore Store { get; }
        private readonly ILogger<DocumentStoreHolder> _logger;

        public DocumentStoreHolder(IOptions<RavenSettings> ravenSettings, ILogger<DocumentStoreHolder> logger)
        {
            _logger = logger;
            var settings = ravenSettings.Value;

            Store = new DocumentStore()
            {
                Urls = new[] { settings.Url },
                Database = settings.Database
            };

            Store.Initialize();

            _logger.LogInformation($"Initialized RavenDB document store for {settings.Database} at {settings.Url}");

            // Create if not exists
            CreateDatabaseIfNotExists();
        }

        private void CreateDatabaseIfNotExists()
        {
            var database = Store.Database;
            var dbRecord = Store.Maintenance.Server.Send(new GetDatabaseRecordOperation(database));

            if (dbRecord == null)
            {
                this._logger.LogInformation("RavenDB database does not exist, creating and seeding with initial data");

                // Create database
                dbRecord = new DatabaseRecordWithEtag();

                var createResult = Store.Maintenance.Server.Send(new CreateDatabaseOperation(dbRecord));


                _logger.LogInformation($"Created database and created dbRecord: {dbRecord}.");
            }

        }
    }
}
