using System;
using System.Collections.Generic;
using System.IO;
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

        private async void ProcessRequestAsyncInner(HttpListenerContext context)
        {
            HttpResponse res = null;
            try {
                res = await ProcessRequestAsync(context);
            }
            catch (Exception ex)
            {
                res = new HttpResponse { StatusCode = HttpStatusCode.InternalServerError, Body = ex.ToString() };
            }

            var body = Encoding.UTF8.GetBytes(res.Body);
            context.Response.StatusCode = (int)res.StatusCode;
            context.Response.ContentLength64 = body.Length;
            using (Stream s = context.Response.OutputStream)
                await s.WriteAsync(body, 0, body.Length);
        }


        protected virtual Task<HttpResponse> ProcessRequestAsync(HttpListenerContext context)
        {
            throw new NotImplementedException();
        }
    }


    public class HttpResponse
    {
        public string Body;
        public HttpStatusCode StatusCode;
    }
}



