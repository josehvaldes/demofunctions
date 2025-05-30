using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Entities;
using Microsoft.DurableTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFunctionIsoApp
{
    public class MessageOrchestration
    {
        [Function("CounterOrchestration")]
        public static async Task Run([OrchestrationTrigger] TaskOrchestrationContext context)
        {
            var entityId = new EntityInstanceId(nameof(StateMessageFunc), "statemessage");

            // Two-way call to the entity which returns a value - awaits the response
            int currentValue = await context.Entities.CallEntityAsync<int>(entityId, "Get");

            if (currentValue < 10)
            {
                // One-way signal to the entity which updates the value - does not await a response
                await context.Entities.SignalEntityAsync(entityId, "Add", 12);
            }
        }
    }
}
