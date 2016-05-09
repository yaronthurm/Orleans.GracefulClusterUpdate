using Orleans;
using Orleans.Runtime.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MigrationService
{
    class Program
    {
        static void Main(string[] args)
        {
            var deploymentId = args[0];
            int gatewayPort = int.Parse(args[1]);
            GrainClient.Initialize(new ClientConfiguration
            {
                DeploymentId = deploymentId,
                Gateways = new[] { new IPEndPoint(IPAddress.Loopback, gatewayPort) }
            });


        }
    }
}
