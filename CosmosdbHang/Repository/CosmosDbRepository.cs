using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace CosmosdbHang.Repository
{
    class CosmosDbRepository
    {
        private readonly AsyncLazy<DocumentClient> _clientFactory;
        private readonly CosmosDBSettings _settings;

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

        internal async Task<bool> ReadDatabase()
        {
            try
            {
                var client = await _clientFactory;
                await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(_settings.DatabaseId)).ConfigureAwait(false);
                return true;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                throw;
            }
        }
    }
}