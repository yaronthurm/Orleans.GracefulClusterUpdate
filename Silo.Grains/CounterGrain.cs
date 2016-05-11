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
        private StorageClient _storageClient = new StorageClient("localhost", 50200);
        private MigrationServiceClient _migrationServiceClient = new MigrationServiceClient("localhost", 50200);
        private int _currentValue;

        public override async Task OnActivateAsync()
        {
            await HandleMigrationLogicIfNeeded();

            var storageRes = await _storageClient.FindAsync("Counters", this.GetPrimaryKeyString());
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
            await _migrationServiceClient.EnsureMigrated(this.GetPrimaryKeyString());
        }

        private async Task<bool> IsMigrated()
        {
            var ret = await _migrationServiceClient.IsMigrated(this.GetPrimaryKeyString());
            return ret;
        }

        public async Task Increment()
        {
            _currentValue++;
            await _storageClient.SaveAsync("Counters", this.GetPrimaryKeyString(), _currentValue.ToString());
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