using System;
using System.Diagnostics;
using System.Web;
using System.Web.Routing;

namespace CosmosdbHang
{
    public class HeartbeatRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new HeartbeatRouteHttpHandler();
        }

        private class HeartbeatRouteHttpHandler : IHttpHandler
        {

            public bool IsReusable { get; } = true;

            public void ProcessRequest(HttpContext context)
            {
                var clientId = Guid.NewGuid(); // client doesn't matter
                var repository = ServiceLocator.Repository;

                Trace.WriteLine("Calling a database");
                var stopwatch = Stopwatch.StartNew();
                
                // let's fake any call to the DB
                repository.GetClient(clientId).ConfigureAwait(false).GetAwaiter().GetResult();

                var msg = $"Database responded in {stopwatch.ElapsedMilliseconds}ms";
                Trace.WriteLine(msg);

                context.Response.Output.WriteLine(msg);
                context.Response.StatusCode = 200;
                context.Response.Flush();
            }
        }
    }
}