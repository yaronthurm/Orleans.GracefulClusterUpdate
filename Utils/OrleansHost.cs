using Orleans.Runtime.Host;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class OrleansHost
    {
        static SiloHost _siloHost;

        Action _onInit;
        AppDomain _siloDomain;

        public OrleansHost() { }

        public OrleansHost(Action onInit)
        {
            _onInit = onInit;
        }

        public void Run(string[] args)
        {
            // Orleans should run in its own AppDomain, we set it up like this
            _siloDomain = AppDomain.CreateDomain("OrleansHost", null,
               new AppDomainSetup()
               {
                   AppDomainInitializer = InitSilo, 
               });

            if (_onInit != null)
                _siloDomain.DoCallBack(new CrossAppDomainDelegate(_onInit));

        }

        public void Stop()
        {
            // We do a clean shutdown in the other AppDomain
            _siloDomain?.DoCallBack(ShutdownSilo);
        }

        

        private static void InitSilo(string[] args)
        {
            _siloHost = new SiloHost(Dns.GetHostName());
            // The Cluster config is quirky and weird to configure in code, so we're going to use a config file
            _siloHost.ConfigFileName = "OrleansConfiguration.xml";

            _siloHost.InitializeOrleansSilo();
            var startedok = _siloHost.StartOrleansSilo();
            if (!startedok)
                throw new SystemException(String.Format("Failed to start Orleans silo '{0}' as a {1} node", _siloHost.Name, _siloHost.Type));
        }

        private void ShutdownSilo()
        {
            if (_siloHost != null)
            {
                _siloHost.Dispose();
                GC.SuppressFinalize(_siloHost);
                _siloHost = null;
            }
        }
    }
}



