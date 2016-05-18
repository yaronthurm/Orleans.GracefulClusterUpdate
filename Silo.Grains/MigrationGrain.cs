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
    public interface IMigrationGrain : IGrainWithStringKey
    {
        Task<bool> IsMigrated();

        Task EnsureMigrated();
    }

    public class MigrationGrain : Grain, IMigrationGrain
    {
        private StorageClient _storageClient = new StorageClient("localhost", 50200);
        private bool _isMigrated;

        public override async Task OnActivateAsync()
        {
            var storageRes = await _storageClient.FindAsync("Migration", this.GetPrimaryKeyString());
            _isMigrated = storageRes.Found;
            await base.OnActivateAsync();
        }

        public Task<bool> IsMigrated()
        {
            return Task.FromResult(_isMigrated);
        }

        public async Task EnsureMigrated()
        {
            if (_isMigrated)
                return;

            await PerformMigration();
        }

        private async Task PerformMigration()
        {
            var httpClient = new HttpClient();
            await httpClient.GetStringAsync($"http://localhost:50200/SiloV1/Deactivate/{this.GetPrimaryKeyString()}");
            _isMigrated = true;
            await _storageClient.SaveAsync("Migration", this.GetPrimaryKeyString(), "1");
        }
    }
}