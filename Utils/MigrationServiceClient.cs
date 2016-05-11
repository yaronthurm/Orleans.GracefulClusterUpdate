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
    public class MigrationServiceClient
    {
        private string _baseUrl;

        public MigrationServiceClient(string host, int port)
        {
            _baseUrl = $"http://{host}:{port}/Migration";
        }

        public async Task EnsureMigrated(string grainKey)
        {
            var fullUrl = $"{_baseUrl}/EnsureMigrated/{grainKey}";
            var res = await new HttpClient().GetAsync(fullUrl);
            if (res.StatusCode == HttpStatusCode.OK)
                return;

            throw new ApplicationException("Failed to ensure grain migration.");
        }

        public async Task<bool> IsMigrated(string grainKey)
        {
            var fullUrl = $"{_baseUrl}/IsMigrated/{grainKey}";
            var res = await new HttpClient().GetAsync(fullUrl);
            if (res.StatusCode == HttpStatusCode.OK)
            {
                var body = await res.Content.ReadAsStringAsync();
                return bool.Parse(body);
            }

            throw new ApplicationException("Failed to acquire grain migration status");
        }
    }
}



