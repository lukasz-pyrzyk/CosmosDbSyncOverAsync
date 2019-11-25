using CosmosdbHang.Repository;

namespace CosmosdbHang
{
    static class ServiceLocator
    {
        public static CosmosDbRepository Repository { get; set; }
    }
}