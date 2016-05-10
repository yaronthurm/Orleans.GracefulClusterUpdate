using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class HttpServer
    {
        private HttpListener _listener;

        public HttpServer(string host, int port, string app)
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://{host}:{port}/{app}/");
        }

        public async Task Start()
        {
            _listener.Start();
            while (true)
            {
                try
                {
                    var context = await _listener.GetContextAsync();
                    Task.Run(() => ProcessRequestAsyncInner(context));
                }
                catch (HttpListenerException) { break; }
                catch (InvalidOperationException) { break; }
            }
        }

        public void Stop()
        {
            _listener.Stop();
        }

        private void ProcessRequestAsyncInner(HttpListenerContext context)
        {
            ProcessRequestAsync(context);
        }


        protected virtual void ProcessRequestAsync(HttpListenerContext context)
        {

        }
    }
}



