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
            // Orleans should run in its own AppDomain, we set it up like this
            AppDomain hostDomain = AppDomain.CreateDomain("OrleansHost", null,
                new AppDomainSetup()
                {
                    AppDomainInitializer = InitSilo
                });

            GrainClient.Initialize(new ClientConfiguration
            {
                Gateways = new[] { new IPEndPoint(IPAddress.Loopback, 27700) }
            });

            var server = new Server();
            server.Start();

            Console.WriteLine("Orleans Silo is running.\nPress Enter to terminate...");
            Console.ReadLine();

            // We do a clean shutdown in the other AppDomain
            hostDomain.DoCallBack(ShutdownSilo);
            server.Stop();
        }

        static void InitSilo(string[] args)
        {
            siloHost = new SiloHost(System.Net.Dns.GetHostName());
            // The Cluster config is quirky and weird to configure in code, so we're going to use a config file
            siloHost.ConfigFileName = "OrleansConfiguration.xml";

            siloHost.InitializeOrleansSilo();
            var startedok = siloHost.StartOrleansSilo();
            if (!startedok)
                throw new SystemException(String.Format("Failed to start Orleans silo '{0}' as a {1} node", siloHost.Name, siloHost.Type));

        }

        static void ShutdownSilo()
        {
            if (siloHost != null)
            {
                siloHost.Dispose();
                GC.SuppressFinalize(siloHost);
                siloHost = null;
            }
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
