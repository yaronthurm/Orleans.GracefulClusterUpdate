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
    public class StorageClient
    {
        private string _baseUrl;

        public StorageClient(string host, int port)
        {
            _baseUrl = $"http://{host}:{port}/Storage";
        }

        public async Task<FindResult> FindAsync(string collectionName, string itemKey)
        {
            var fullUrl = $"{_baseUrl}/Get/{collectionName}_{itemKey}";
            var storageRes = await new HttpClient().GetAsync(fullUrl);
            if (storageRes.StatusCode == HttpStatusCode.OK)
                return new FindResult { Found = true, Body = await storageRes.Content.ReadAsStringAsync() };

            if (storageRes.StatusCode == HttpStatusCode.NotFound)
                return new FindResult { Found = false, Body = null };

            throw new ApplicationException("Could not find item");
        }

        public async Task SaveAsync(string collectionName, string itemKey, string value)
        {
            var fullUrl = $"{_baseUrl}/Save/{collectionName}_{itemKey}/{value}";
            var storageRes = await new HttpClient().GetAsync(fullUrl);
            if (storageRes.StatusCode == HttpStatusCode.OK)
                return;

            throw new ApplicationException("Failed to save item");
        }
    }
}



