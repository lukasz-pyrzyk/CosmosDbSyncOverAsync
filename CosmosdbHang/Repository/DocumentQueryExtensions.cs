using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace CosmosdbHang.Repository
{
    static class DocumentQueryExtensions
    {
        public static async Task<TQueryItem> ExecuteFirstOrDefault<TQueryItem>(this FeedIterator<TQueryItem> iterator, string operationName)
        {
            Trace.WriteLine($"Executing {operationName}");
            var feedResponse = await iterator.ReadNextAsync().ConfigureAwait(false);
            Trace.WriteLine($"{operationName} finished with status code {feedResponse.StatusCode}. Request charge is {feedResponse.RequestCharge}.");

            return feedResponse.FirstOrDefault();
        }
    }
}