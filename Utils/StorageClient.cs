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
            var fullUrl = $"{_baseUrl}/Get/{JoinUnderscore(collectionName, itemKey)}";
            var storageRes = await new HttpClient().GetAsync(fullUrl);
            if (storageRes.StatusCode == HttpStatusCode.OK)
                return new FindResult { Found = true, Body = await storageRes.Content.ReadAsStringAsync() };

            if (storageRes.StatusCode == HttpStatusCode.NoContent)
                return new FindResult { Found = false, Body = null };

            throw new ApplicationException("Could not find item");
        }

        public async Task SaveAsync(string collectionName, string itemKey, string value)
        {
            var fullUrl = $"{_baseUrl}/Save/{JoinUnderscore(collectionName, itemKey)}/{value}";
            var storageRes = await new HttpClient().GetAsync(fullUrl);
            if (storageRes.StatusCode == HttpStatusCode.OK)
                return;

            throw new ApplicationException("Failed to save item");
        }

        /// <summary>
        /// This method exists to allow joining two string using '_' as separator without the risk
        /// of two different strings joining to the same string.
        /// e.g. without this method, joining "a_" and "b" will give the same result as 
        /// joining "a" and "_b". Both cases will yield "a_b".
        /// Using this method however, "a_" and "b" will yield "a|1_b", while joining 
        /// "a" and "_b" will yield "a_|1b" 0 different results.
        /// If the user will pass "|" the method will transfer it to "|9" so even here the 
        /// output is correct since the user can never "full" the output by either "|" or "_".
        /// e.g. "a|1" and "b" will become "a|91_b" - the underscore char can only apear between the two values
        /// and pipe ("|") can only apear as |1 (which corresponds to _ passed by the user) or |9 (which
        /// corresponds to | passed by the user)
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        private static string JoinUnderscore(string value1, string value2)
        {
            Func<string, string> escape = x => x.Replace("|", "|9").Replace("_", "|1");
            var ret = $"{escape(value1)}_{escape(value2)}";
            return ret;
        }


        public class FindResult
        {
            public bool Found;
            public string Body;       
        }
    }
}



