using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace CosmosdbHang.Repository
{
    class CosmosDbRepository
    {
        private readonly AsyncLazy<DocumentClient> _clientFactory;
        private readonly CosmosDBSettings _settings;
        private Uri _collectionUri;

        internal CosmosDbRepository(CosmosDBSettings settings)
        {
            _settings = settings;

            _clientFactory = new AsyncLazy<DocumentClient>(async () =>
            {
                var policy = new ConnectionPolicy
                {
                    ConnectionMode = _settings.UseDirectConnectionMode ? ConnectionMode.Direct : ConnectionMode.Gateway,
                    ConnectionProtocol = Protocol.Tcp,
                    UseMultipleWriteLocations = true
                };

                policy.PreferredLocations.Add(settings.Region);

                var client = new DocumentClient(new Uri(_settings.Endpoint), _settings.Key, policy);

                await PrepareDatabase(client).ConfigureAwait(false);
                _collectionUri = PrepareCollection(client);

                return client;
            });
        }

        private async Task<string> PrepareDatabase(DocumentClient client)
        {
            try
            {
                var response = await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(_settings.DatabaseId)).ConfigureAwait(false);
                response.Log(nameof(PrepareDatabase));
                return _settings.DatabaseId;
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    Trace.WriteLine($"Database {_settings.DatabaseId} doesn't exist!");
                }

                throw;
            }
        }

        private Uri PrepareCollection(DocumentClient client)
        {
            var collection = client.CreateDocumentCollectionQuery(UriFactory.CreateDatabaseUri(_settings.ContainerId)).Where(c => c.Id == _settings.ContainerId).ToArray().FirstOrDefault();
            if (collection is null)
            {
                Trace.WriteLine($"Collection {_settings.ContainerId} does not exist!");
                throw new Exception($"Collection {_settings.ContainerId} does not exist");
            }

            return UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, _settings.ContainerId);
        }

        internal async Task<ClientEntity> GetClient(Guid clientId)
        {
            try
            {
                var dbClient = await _clientFactory;
                using (var query = dbClient.CreateDocumentQuery<ClientEntity>(_collectionUri, new FeedOptions { PartitionKey = new PartitionKey(clientId.ToString()) })
                    .Where(x => x.ClientId == clientId)
                    .AsDocumentQuery())
                {

                    var client = await query.ExecuteFirstOrDefault(nameof(GetClient)).ConfigureAwait(false);
                    return client;
                }
            }
            catch (DocumentClientException ex)
            {
                Trace.WriteLine($"Unable to get the client {clientId} from storage. Exception: {ex}");
                return null;
            }
        }
    }
}