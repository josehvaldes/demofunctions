using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.DurableTask.Client.Entities;
using Microsoft.DurableTask.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EntityFunctionIsoApp
{
    public class HttpTriggerEntityFunction
    {

        [Function("HttpFunctionDemoWriter")]
        public static Task Run(
            [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req,
            [DurableClient] DurableTaskClient client)
        {
            // Entity operation input comes from the queue message content. 
            var entityId = new EntityInstanceId(nameof(StateMessageFunc), "statemessage");
            
            var input = req.Query.ContainsKey("input")? req.Query["input"].ToString():string.Empty;

            if (!string.IsNullOrEmpty(input)) 
            {
                int amount = int.Parse(input);
                return client.Entities.SignalEntityAsync(entityId, "Add", amount);
            }

            return Task.CompletedTask;
        }


        [Function("HttpFunctionDemoReader")]
        public static async Task<HttpResponseData> RunReader(
            [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req,
            [DurableClient] DurableTaskClient client)
        {
            try 
            {
                // Entity operation input comes from the queue message content. 
                var entityId = new EntityInstanceId(nameof(StateMessageFunc), "statemessage");

                var entity = await client.Entities.GetEntityAsync<StateMessageFunc>(entityId);

                //EntityMetadata<JObject>? entity = await client.Entities.GetEntityAsync<JObject>(entityId);

                if (entity is null)
                {
                    return req.CreateResponse(HttpStatusCode.NotFound);
                }

                HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(entity);

                return response;
            } 
            catch (Exception e) 
            {
                throw e;
            }            
        }
    }
}
