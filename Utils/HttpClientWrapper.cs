using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class HttpClientWrapper
    {
        private string _baseUrl;

        public HttpClientWrapper(string host, int port, string app)
        {
            _baseUrl = $"http://{host}:{port}/{app}/";
        }

        public async Task<FindResult> FindResourceAsync(string pathToResource)
        {
            var fullUrl = $"{_baseUrl}/{pathToResource}";
            var storageRes = await new HttpClient().GetAsync(fullUrl);
            if (storageRes.StatusCode != System.Net.HttpStatusCode.OK && storageRes.StatusCode != System.Net.HttpStatusCode.NotFound)
                throw new ApplicationException("Could not fetch resource");
            if (storageRes.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return new FindResult
                {
                    Found = true,
                    Body = await storageRes.Content.ReadAsStringAsync()
                };
            }
            else
                return new FindResult
                {
                    Found = false,
                    Body = null
                };
        }
    }


    public class FindResult
    {
        public string Body;
        public bool Found;
    }
}



