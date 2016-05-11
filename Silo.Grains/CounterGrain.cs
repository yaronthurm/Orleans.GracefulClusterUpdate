using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Silo.Grains
{
    public static class GlobalMigrationConfig
    {
        public static bool IsMigrationOn;
        public static bool IsOldCluster;
    }

    public interface ICounterGrain : IGrainWithStringKey
    {
        Task Increment();
        Task<int> GetValue();
        Task Deactivate();
    }

    public class CounterGrain : Grain, ICounterGrain
    {
        private HttpClientWrapper _storageClient = new HttpClientWrapper("localhost", 50200, "Storage");
        private int _currentValue;

        public override async Task OnActivateAsync()
        {
            await HandleMigrationLogicIfNeeded();

            var storageRes = await _storageClient.FindResourceAsync($"Get/Counters_{this.GetPrimaryKeyString()}");
            if (storageRes.Found) _currentValue = int.Parse(storageRes.Body);

            await base.OnActivateAsync();
        }

        private async Task HandleMigrationLogicIfNeeded()
        {
            if (!GlobalMigrationConfig.IsMigrationOn)
                return;
            
            if (GlobalMigrationConfig.IsOldCluster) {
                if (await IsMigrated())
                    throw new ApplicationException("Grain already migrated. Cannot be reactivated.");
            }
            else
            {
                await EnsureMigrated();
            }
        }

        private async Task EnsureMigrated()
        {
            string grainKey = this.GetPrimaryKeyString();
            var httpClient = new HttpClient();
            var res = await httpClient.GetAsync($"http://localhost:50200/Migration/EnsureMigrated/{grainKey}");
            if (res.StatusCode != System.Net.HttpStatusCode.OK)
                throw new ApplicationException("Failed to migrate grain");
        }

        private async Task<bool> IsMigrated()
        {
            string grainKey = this.GetPrimaryKeyString();
            var httpClient = new HttpClient();
            var res = await httpClient.GetAsync($"http://localhost:50200/Migration/IsMigrated/{grainKey}");
            if (res.StatusCode != System.Net.HttpStatusCode.OK)
                throw new ApplicationException("Failed to validate grain migration status");
            var body = await res.Content.ReadAsStringAsync();
            return bool.Parse(body);
        }

        public async Task Increment()
        {
            _currentValue++;
            await _storageClient.FindResourceAsync($"Save/Counters_{this.GetPrimaryKeyString()}/{_currentValue}");
        }

        public Task<int> GetValue()
        {
            return Task.FromResult(_currentValue);
        }

        public Task Deactivate()
        {
            this.DeactivateOnIdle();
            return TaskDone.Done;
        }
    }
}