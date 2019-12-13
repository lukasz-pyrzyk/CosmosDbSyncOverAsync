namespace CosmosdbHang.Repository
{
    class CosmosDBSettings
    {
        public string Endpoint { get; }
        public string Key { get; }
        public string DatabaseId { get; }
        public string ContainerId { get; set; }
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
        }
    }
}