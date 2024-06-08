using Microsoft.Extensions.Configuration;
using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

//reference: https://docs.microsoft.com/en-us/azure/event-hubs/event-hubs-dotnet-standard-getstarted-send
namespace WorkingWithEventHub
{
    public class Program
    {
        private static IConfigurationRoot _configuration;

        // connection string to the Event Hubs namespace
        private static string _eventHubConnectionString;// = "<EVENT HUBS NAMESPACE - CONNECTION STRING>";

        // name of the event hub
        private static string _eventHubName; //  = "<EVENT HUB NAME>";

        private static EventHubProducerClient producerClient;

        private static int _numberOfEvents = 25;

        private static Random random = new Random();

        public static async Task Main(string[] args)
        {
            BuildOptions();
            Console.WriteLine("Hello World");

            _eventHubConnectionString = _configuration["EventHub:ConnectionString"]; 
            _eventHubName = _configuration["EventHub:Name"];

            // Create a producer client that you can use to send events to an event hub
            producerClient = new EventHubProducerClient(_eventHubConnectionString, _eventHubName);

            // Create a batch of events 
            using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();

            for (int i = 1; i <= _numberOfEvents; i++)
            {
                var log = GenerateNewLogMessage();
                if (!eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes($"Event {i}: {log}"))))
                {
                    // if it is too large for the batch
                    throw new Exception($"Event {i} is too large for the batch and cannot be sent.");
                }
            }

            try
            {
                // Use the producer client to send the batch of events to the event hub
                await producerClient.SendAsync(eventBatch);
                Console.WriteLine($"A batch of {_numberOfEvents} events has been published.");
            }
            finally
            {
                await producerClient.DisposeAsync();
            }
        }

        private static string GenerateNewLogMessage()
        { 
            var messages = new List<string>() {
                "Initiating changes for package KB5012599. Current state is Absent. Target state is Installed. Client id: UpdateAgentLCU.",
                "A reboot is necessary before package KB5013624 can be changed to the Installed state.",
                "A reboot is necessary before package KB5013942 can be changed to the Installed state.",
                "A user's local group membership was enumerated.",
                "Special privileges assigned to new logon.",
                "An account was successfully logged on.",
                "Credential Manager credentials were read.",
                "Cryptographic operation.",
                "An attempt was made to query the existence of a blank password for an account.",
                "Code Integrity determined that the page hashes of an image file are not valid. The file could be improperly signed without page hashes or corrupt due to unauthorized modification. The invalid hashes could indicate a potential disk device error."
            };
            return messages[random.Next(0, 9)];
        }

        private static void BuildOptions()
        {
            _configuration = ConfigurationBuilderSingleton.ConfigurationRoot;
        }
    }
}
