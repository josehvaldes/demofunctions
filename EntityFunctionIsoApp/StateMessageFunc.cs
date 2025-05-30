using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions;
using System.Runtime.CompilerServices;

namespace EntityFunctionIsoApp
{
    public class StateMessageFunc
    {
        public int CurrentValue { get; set; }

        public void Add(int amount) => this.CurrentValue += amount;

        public void Reset() => this.CurrentValue = 0;

        public int Get() => this.CurrentValue;

        [Function(nameof(StateMessageFunc))]
        public static Task RunEntityAsync([EntityTrigger] TaskEntityDispatcher dispatcher)
        {
            
            return dispatcher.DispatchAsync<StateMessageFunc>();
        }
    }
}
