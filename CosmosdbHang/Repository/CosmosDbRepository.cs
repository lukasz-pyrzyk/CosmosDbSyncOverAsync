using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace CosmosdbHang.Repository
{
    class CosmosDbRepository : IDisposable
    {
        private readonly AsyncLazy<Database> clientFactory;
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
            clientFactory = new AsyncLazy<Database>(async () =>
            {
                var database = await CheckIfDatabaseExists(_databaseClient).ConfigureAwait(false);
                return database;
            });
        }

        private async Task<Database> CheckIfDatabaseExists(CosmosClient client)
        {
            var database = client.GetDatabase(_settings.DatabaseId);
            var read = await database.ReadStreamAsync().ConfigureAwait(false);
            if (read.StatusCode == HttpStatusCode.NotFound)
            {
                throw new Exception($"CosmosDB database {_settings.DatabaseId} does not exist!");
            }

            return database;
        }

        public async Task ReadDatabase()
        {
            try
            {
                var client = await clientFactory;
                await client.ReadAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void Dispose()
        {
            _databaseClient?.Dispose();
        }
    }
}