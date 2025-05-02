using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace OrchestrationFunction
{
    public static class StartHello
    {
        [Function(nameof(StartHello))]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] TaskOrchestrationContext context)
        {
            ILogger logger = context.CreateReplaySafeLogger(nameof(StartHello));
            logger.LogInformation("Saying StartHello.");
            var outputs = new List<string>();

            //chaining pattern
            // Replace name and input with values relevant for your Durable Functions Activity
            //outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "Tokyo"));
            //outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "Seattle"));
            //outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "London"));

            //fan-in/fan-out pattern
            var tasks = new Task<string>[3];            
            tasks[0] = context.CallActivityAsync<string>(nameof(SayHello), "Tokyo");
            tasks[1] = context.CallActivityAsync<string>(nameof(SayHello), "Seattle");
            tasks[2] = context.CallActivityAsync<string>(nameof(SayHello), "London");

            await Task.WhenAll(tasks);
            foreach (var task in tasks)
            {
                outputs.Add(task.Result);
            }

            logger.LogInformation("End StartHello.");
            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }

        [Function(nameof(SayHello))]
        public static string SayHello([ActivityTrigger] string name, FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("SayHello");
            logger.LogInformation("Saying hello to {name}.", name);
            return $"Hello {name}!";
        }

        [Function("StartHello_HttpStart")]
        public static async Task<HttpResponseData> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
            [DurableClient] DurableTaskClient client,
            FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("Function1_HttpStart");

            // Function input comes from the request content.
            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
                nameof(StartHello));

            logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

            // Returns an HTTP 202 response with an instance management payload.
            // See https://learn.microsoft.com/azure/azure-functions/durable/durable-functions-http-api#start-orchestration
            return await client.CreateCheckStatusResponseAsync(req, instanceId);
        }
    }
}
