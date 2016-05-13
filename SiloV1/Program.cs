using Orleans;
using Orleans.Runtime.Configuration;
using Orleans.Runtime.Host;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Silo.Grains;
using Utils;

namespace SiloV1
{
    class Program
    {

        static SiloHost siloHost;

        static void Main(string[] args)
        {
            var silo = new OrleansHost(() =>
            {
                GlobalMigrationConfig.IsMigrationOn = true;
                GlobalMigrationConfig.IsOldCluster = true;
            });
            silo.Run(args);

            GrainClient.Initialize(new ClientConfiguration { Gateways = new[] { new IPEndPoint(IPAddress.Loopback, 27500) } });

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
        public Server() : base("localhost", 50200, "SiloV1") { }

        protected override async Task<HttpResponse> ProcessRequestAsync(HttpListenerContext context)
        {
            // Expected "/SiloV1/<method>/<id>"
            var path = context.Request.RawUrl.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            if (path.Length != 3)
                return new HttpResponse { StatusCode = HttpStatusCode.BadRequest, Body = "Invalid url format. Expected /SiloV1/<method>/<id>" };

            var grain = GrainClient.GrainFactory.GetGrain<ICounterGrain>(path[2]);
            switch (path[1])
            {
                case "Increment":
                    await grain.Increment();
                    return new HttpResponse { StatusCode = HttpStatusCode.OK, Body = "" };

                case "GetValue":
                    var val = await grain.GetValue();
                    return new HttpResponse { StatusCode = HttpStatusCode.OK, Body = val.ToString() };

                case "Deactivate":
                    await grain.Deactivate();
                    return new HttpResponse { StatusCode = HttpStatusCode.OK, Body = "" };


                default: return new HttpResponse { StatusCode = HttpStatusCode.BadRequest, Body = "Invalid method" };
            }
        }
    }
}
