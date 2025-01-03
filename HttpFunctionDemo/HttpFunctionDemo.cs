using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace HttpFunctionDemo
{
    public class HttpFunctionDemo
    {
        private readonly ILogger<HttpFunctionDemo> _logger;

        public HttpFunctionDemo(ILogger<HttpFunctionDemo> logger)
        {
            _logger = logger;
        }

        [Function("HttpFunctionDemo")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
