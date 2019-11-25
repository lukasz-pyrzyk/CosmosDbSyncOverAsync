using System;
using System.Web;
using System.Web.Routing;
using CosmosdbHang.Repository;

namespace CosmosdbHang
{
    public class Global : HttpApplication
    {
        public Global()
        {
            RegisterCosmosDbClient();
            RegisterEndpoint();
        }

        private static void RegisterCosmosDbClient()
        {
            var endpoint = Environment.GetEnvironmentVariable("CosmosdbHang-endpoint");
            var key = Environment.GetEnvironmentVariable("CosmosdbHang-key");
            var databaseId = Environment.GetEnvironmentVariable("CosmosdbHang-databaseId");
            var containerId = Environment.GetEnvironmentVariable("CosmosdbHang-containerId");
            var region = Environment.GetEnvironmentVariable("CosmosdbHang-region");
            var directMode = true;

            var settings = new CosmosDBSettings(endpoint, key, databaseId, containerId, region, directMode);

            ServiceLocator.Repository = new CosmosDbRepository(settings);
        }

        private static void RegisterEndpoint()
        {
            RouteTable.Routes.Add(new Route("heartbeat", new HeartbeatRouteHandler()));
        }
    }
}