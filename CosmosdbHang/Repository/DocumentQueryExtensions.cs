using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace CosmosdbHang.Repository
{
    static class DocumentQueryExtensions
    {
        internal static void Log<T>(this ResourceResponse<T> response, string operationName) where T : Microsoft.Azure.Documents.Resource, new()
        {
            Trace.WriteLine($"{operationName} finished. Request charge is {response.RequestCharge}.");
        }

        internal static async Task<TQueryItem> ExecuteFirstOrDefault<TQueryItem>(this IDocumentQuery<TQueryItem> documentQuery, string operationName)
        {
            Trace.WriteLine($"Executing {operationName}");
            var feedResponse = await documentQuery.ExecuteNextAsync<TQueryItem>().ConfigureAwait(false);
            Trace.WriteLine($"{operationName} finished. Request charge is {feedResponse.RequestCharge}.");

            return feedResponse.FirstOrDefault();
        }
    }
}