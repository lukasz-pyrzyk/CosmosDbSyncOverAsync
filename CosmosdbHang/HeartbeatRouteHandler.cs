using System;
using System.Diagnostics;
using System.Threading;
using System.Web;
using System.Web.Routing;

namespace CosmosdbHang
{
    public class HeartbeatRouteHandler : IRouteHandler
    {
        private static int _requestNumber;

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new HeartbeatRouteHttpHandler();
        }

        private class HeartbeatRouteHttpHandler : IHttpHandler
        {
            public bool IsReusable { get; } = true;

            public void ProcessRequest(HttpContext context)
            {

                int requestNumber = Interlocked.Increment(ref _requestNumber);
                Trace.WriteLine($"Request #{requestNumber}, calling a database");
                var stopwatch = Stopwatch.StartNew();
                
                // let's fake any call to the DB
                var repository = ServiceLocator.Repository;
                repository.GetDb().ConfigureAwait(false).GetAwaiter().GetResult();
                
                var msg = $"Request #{requestNumber}, database responded in {stopwatch.ElapsedMilliseconds}ms";
                Trace.WriteLine(msg);

                context.Response.Output.WriteLine(msg);
                context.Response.StatusCode = 200;
                context.Response.Flush();
            }
        }
    }
}