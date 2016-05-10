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

namespace SiloV2
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
                Gateways = new[] { new IPEndPoint(IPAddress.Loopback, 27600) }
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



    public class Server: HttpServer
    {
        public Server(): base("localhost", 50200, "SiloV2") { }

        protected override async Task ProcessRequestAsync(HttpListenerContext context)
        {
            switch (context.Request.RawUrl)
            {
                case "/SiloV2/Increment":
                    await GrainClient.GrainFactory.GetGrain<ICounterGrain>(Guid.NewGuid()).Increment();
                    break;
            }
        }
    }
}
