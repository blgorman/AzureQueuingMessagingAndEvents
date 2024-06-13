using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


namespace FileProcessingFunctions
{
    public class ProcessStreamAlert
    {
        private readonly ILogger<ProcessStreamAlert> _logger;

        public ProcessStreamAlert(ILogger<ProcessStreamAlert> logger)
        {
            _logger = logger;
        }

        [Function("ProcessStreamAlert")]
        public async Task Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("Triggered Function from stream Started");
            

            // Extract the body from the request
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            _logger.LogInformation(requestBody);
            if (string.IsNullOrWhiteSpace(requestBody)) 
            {
                _logger.LogError("NULL REQUEST DATA DETECTED");
                return; 
            }

            List<WeatherData> data = JsonConvert.DeserializeObject<List<WeatherData>>(requestBody);
            //if (data == null) 
            //{
            //    _logger.LogError("Could not deserialize weather data");
            //}

            foreach (var wd in data)
            {
                _logger.LogWarning($"Weather ALERT: Wind Speed of {wd.WindSpeed} going in {wd.WindDirection} detected in {wd.City}");

                //send alert, call api, etc.
            }
            

            _logger.LogInformation("Triggered Function from stream completed");
        }
    }
}
