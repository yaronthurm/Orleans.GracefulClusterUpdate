using Orleans;
using Orleans.Runtime.Configuration;
using Orleans.Runtime.Host;
using Silo.Grains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace MigrationService
{
    class Program
    {
        static SiloHost siloHost;

        static void Main(string[] args)
        {
            var silo = new OrleansHost();
            silo.Run(args);

            GrainClient.Initialize(new ClientConfiguration { Gateways = new[] { new IPEndPoint(IPAddress.Loopback, 27700) } });

            var server = new Server();
            server.Start();

            Console.WriteLine("Orleans Silo is running.\nPress Enter to terminate...");
            Console.ReadLine();

            silo.Stop();
            server.Stop();
        }
    }



    public class Server : HttpServer
    {
        public Server() : base("localhost", 50200, "Migration") { }

        protected override async Task<HttpResponse> ProcessRequestAsync(HttpListenerContext context)
        {
            // Expected "/Migration/<method>/<id>"
            var path = context.Request.RawUrl.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            if (path.Length != 3)
                return new HttpResponse { StatusCode = HttpStatusCode.BadRequest, Body = "Invalid url format. Expected /Migration/<method>/<id>" };

            var grain = GrainClient.GrainFactory.GetGrain<IMigrationGrain>(path[2]);
            switch (path[1])
            {
                case "EnsureMigrated":
                    await grain.EnsureMigrated();
                    return new HttpResponse { StatusCode = HttpStatusCode.OK, Body = "" };

                case "IsMigrated":
                    var val = await grain.IsMigrated();
                    return new HttpResponse { StatusCode = HttpStatusCode.OK, Body = val.ToString() };

                default: return new HttpResponse { StatusCode = HttpStatusCode.BadRequest, Body = "Invalid method" };
            }
        }
    }
}
