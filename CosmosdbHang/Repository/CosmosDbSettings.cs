using System;
using System.Linq;
using System.Reflection;
using Microsoft.Azure.Cosmos;

namespace CosmosdbHang.Repository
{
    class CosmosDBSettings
    {
        public string Endpoint { get; }
        public string Key { get; }
        public string DatabaseId { get; }
        public string ContainerId { get; }
        public string Region { get; }
        public bool UseDirectConnectionMode { get; }

        public CosmosDBSettings(string endpoint, string key, string databaseId, string containerId, string region, bool useDirectMode)
        {
            Endpoint = endpoint;
            Key = key;
            DatabaseId = databaseId;
            ContainerId = containerId;
            Region = region;
            UseDirectConnectionMode = useDirectMode;

            ValidateRegion(Region);
        }

        private void ValidateRegion(string region)
        {
            var allowedLocations = GetAllowedRegions();
            if (!allowedLocations.Contains(region))
            {
                throw new Exception($"Region {region} is not recognized by CosmosDB SDK");
            }

        }

        private static string[] GetAllowedRegions()
        {
            return typeof(Regions)
                .GetFields(BindingFlags.Static | BindingFlags.Public)
                .Where(x => x.IsLiteral && x.FieldType == typeof(string))
                .Select(x => (string)x.GetRawConstantValue()).ToArray();
        }
    }
}