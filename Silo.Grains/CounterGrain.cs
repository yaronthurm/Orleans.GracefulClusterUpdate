using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Silo.Grains
{
    public interface ICounterGrain : IGrainWithStringKey
    {
        Task Increment();
        Task<int> GetValue();
        Task Deactivate();
    }

    public class CounterGrain : Grain, ICounterGrain
    {
        private int _currentValue;

        public override Task OnActivateAsync()
        {
            _currentValue = 0; // TODO: Fetch from storage
            return base.OnActivateAsync();
        }


        public Task Increment()
        {
            _currentValue++;
            // TODO: Persist to storage
            return TaskDone.Done;
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