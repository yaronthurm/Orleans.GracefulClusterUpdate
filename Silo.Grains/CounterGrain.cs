using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Utils;
using Orleans.CodeGeneration;
using System.Reflection;

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

    public class CounterGrain : Grain, ICounterGrain, IGrainInvokeInterceptor
    {
        private StorageClient _storageClient = new StorageClient("localhost", 50200);
        private MigrationServiceClient _migrationServiceClient = new MigrationServiceClient("localhost", 50200);
        private int _currentValue;
        private bool _isFirstActivation = true;

        public async Task<object> Invoke(MethodInfo method, InvokeMethodRequest request, IGrainMethodInvoker invoker)
        {
            if (_isFirstActivation)
            {
                if (!IsDeactivating(method))
                {
                    await HandleMigrationLogicIfNeeded();
                    await Initialize();
                }
                _isFirstActivation = false;
            }

            var ret = await invoker.Invoke(this, request);
            return ret;
        }

        private async Task Initialize()
        {
            var storageRes = await _storageClient.FindAsync("Counters", this.GetPrimaryKeyString());
            if (storageRes.Found) _currentValue = int.Parse(storageRes.Body);
        }

        private bool IsDeactivating(MethodInfo method)
        {
            var ret = method.Name == ((Func<Task>)this.Deactivate).Method.Name;
            return ret;
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