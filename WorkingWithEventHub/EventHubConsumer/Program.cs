using Microsoft.Extensions.Configuration;
using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;

namespace EventHubConsumer
{
    public class Program
    {
        private static IConfigurationRoot _configuration;
        // connection string to the Event Hubs namespace
        private static string _eventHubConnectionString;// = "<EVENT HUBS NAMESPACE - CONNECTION STRING>";

        // name of the event hub
        private static string _eventHubName; //  = "<EVENT HUB NAME>";

        private static string _blobStorageConnectionString; // = "<AZURE STORAGE CONNECTION STRING>";
        private static string _blobContainerName; // = "<BLOB CONTAINER NAME>";

        private static EventProcessorClient _processor;
        private static BlobContainerClient _storageClient;

        private static Random r = new Random();

        public static async Task Main(string[] args)
        {
            BuildOptions();
            Console.WriteLine("Hello World");

            _eventHubConnectionString = _configuration["EventHub:ConnectionString"];
            _eventHubName = _configuration["EventHub:Name"];
            _blobStorageConnectionString = _configuration["Storage:ConnectionString"];
            _blobContainerName = _configuration["Storage:ContainerName"];

            // Read from the default consumer group: $Default
            string consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;
            _storageClient = new BlobContainerClient(_blobStorageConnectionString
                                                            , _blobContainerName);

            // Create an event processor client to process events in the event hub
            _processor = new EventProcessorClient(_storageClient, consumerGroup
                                    , _eventHubConnectionString, _eventHubName);

            // Register handlers for processing events and handling errors
            _processor.ProcessEventAsync += ProcessEventHandler;
            _processor.ProcessErrorAsync += ProcessErrorHandler;

            // Start the processing
            await _processor.StartProcessingAsync();

            // Wait for 30 seconds for the events to be processed
            await Task.Delay(TimeSpan.FromSeconds(30));

            // Stop the processing
            await _processor.StopProcessingAsync();
        }

        private static async Task ProcessEventHandler(ProcessEventArgs eventArgs)
        {
            var eventArgsMessageData = eventArgs.Data.Body.ToArray();
            var eventArgsMessageString = Encoding.UTF8.GetString(eventArgsMessageData);
            
            // Write the body of the event to the console window
            Console.WriteLine($"\tReceived event: {eventArgsMessageString}");

            // Save the data to a text file at Storage:
            var fileName = $"{DateTime.Now.ToString("yyyy.MM.dd_")}{r.Next(1000, 1999)}.txt";
            var blobClient = _storageClient.GetBlobClient(fileName);

            using (var ms = new MemoryStream(eventArgsMessageData))
            {
                await blobClient.UploadAsync(ms, true);
            }

            // Update checkpoint in the consumer group so that the app processes
            // only new events the next time it's run
            await eventArgs.UpdateCheckpointAsync(eventArgs.CancellationToken);
        }

        private static Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
        {
            // Write details about the error to the console window
            Console.WriteLine($"\tPartition '{ eventArgs.PartitionId}': an unhandled exception was encountered. This was not expected to happen.");
            Console.WriteLine(eventArgs.Exception.Message);
            return Task.CompletedTask;
        }

        private static void BuildOptions()
        {
            _configuration = ConfigurationBuilderSingleton.ConfigurationRoot;
        }
    }
}
