using System.Diagnostics;
using Microsoft.Azure.Documents.Client;

namespace CosmosdbHang.Repository
{
    static class DocumentQueryExtensions
    {
        internal static void Log<T>(this ResourceResponse<T> response, string operationName) where T : Microsoft.Azure.Documents.Resource, new()
        {
            Trace.WriteLine($"{operationName} finished. Request charge is {response.RequestCharge}.");
        }
    }
}