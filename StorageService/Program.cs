using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace StorageService
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new Server();
            server.Start();

            Console.WriteLine("Started Storage service.\nPress Enter to terminate...");
            Console.ReadLine();
        }
    }


    public class Server : HttpServer
    {
        public static ConcurrentDictionary<string, string> Data = new ConcurrentDictionary<string, string>();

        public Server() : base("localhost", 50200, "Storage") { }

        protected override async Task<HttpResponse> ProcessRequestAsync(HttpListenerContext context)
        {
            // Expected "/Storage/Save/<id>/<value>"  or
            //          "/Storage/Get/<id>

            var path = context.Request.RawUrl.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            if (path.Length < 2) return new HttpResponse { StatusCode = HttpStatusCode.BadRequest, Body = "Invalid url format" };

            var method = path[1];
            switch (method)
            {
                case "Save":
                    if (path.Length != 4) return new HttpResponse { StatusCode = HttpStatusCode.BadRequest, Body = "Invalid url format. Expected /Storage/Save/<id>/<value>" };
                    Data[path[2]] = path[3];
                    return new HttpResponse { StatusCode = HttpStatusCode.OK, Body = "" };

                case "Get":
                    if (path.Length != 3) return new HttpResponse { StatusCode = HttpStatusCode.BadRequest, Body = "Invalid url format. Expected /Storage/Get/<id>" };
                    string val;
                    if (Data.TryGetValue(path[2], out val))
                        return new HttpResponse { StatusCode = HttpStatusCode.OK, Body = val.ToString() };
                    else
                        return new HttpResponse { StatusCode = HttpStatusCode.NotFound, Body = "" };

                default: return new HttpResponse { StatusCode = HttpStatusCode.BadRequest, Body = "Invalid method" };
            }
        }
    }
}
