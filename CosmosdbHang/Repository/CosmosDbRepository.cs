using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace CosmosdbHang.Repository
{
    class CosmosDbRepository : IDisposable
    {
        private readonly AsyncLazy<Container> _containerFactory;
        private readonly CosmosDBSettings _settings;
        private readonly CosmosClient _databaseClient;

        internal CosmosDbRepository(CosmosDBSettings settings)
        {
            _settings = settings;
            var options = new CosmosClientOptions
            {
                ConnectionMode = _settings.UseDirectConnectionMode ? ConnectionMode.Direct : ConnectionMode.Gateway,
                ApplicationRegion = _settings.Region
            };

            _databaseClient = new CosmosClient(_settings.Endpoint, _settings.Key, options);
            _containerFactory = new AsyncLazy<Container>(async () =>
            {
                await CheckIfDatabaseExists(_databaseClient).ConfigureAwait(false);
                var container = await CheckIfContainerExists(_databaseClient).ConfigureAwait(false);
                return container;
            });
        }

        private async Task CheckIfDatabaseExists(CosmosClient client)
        {
            var database = client.GetDatabase(_settings.DatabaseId);
            var read = await database.ReadStreamAsync().ConfigureAwait(false);
            if (read.StatusCode == HttpStatusCode.NotFound)
            {
                throw new Exception($"CosmosDB database {_settings.DatabaseId} does not exist!");
            }
        }

        private async Task<Container> CheckIfContainerExists(CosmosClient client)
        {
            var container = client.GetContainer(_settings.DatabaseId, _settings.ContainerId);
            var response = await container.ReadContainerStreamAsync().ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new Exception($"CosmosDB container {_settings.ContainerId} does not exist!");
            }

            return container;
        }

        internal async Task<ClientEntity> GetClient(Guid clientId)
        {
            try
            {
                var dbClient = await _containerFactory;
                var partitionKey = new PartitionKey(clientId.ToString());
                var query = dbClient
                    .GetItemLinqQueryable<ClientEntity>(requestOptions: new QueryRequestOptions { PartitionKey = partitionKey })
                    .Where(x => x.ClientId == clientId)
                    .ToFeedIterator();

                var client = await query.ExecuteFirstOrDefault(nameof(GetClient)).ConfigureAwait(false);
                return client;
            }
            catch (CosmosException ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public void Dispose()
        {
            _databaseClient?.Dispose();
        }
    }
}