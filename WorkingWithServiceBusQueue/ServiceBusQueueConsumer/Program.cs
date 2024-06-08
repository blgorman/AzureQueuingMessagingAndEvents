using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using MySolutionObjectModels;
using Newtonsoft.Json;
using System.Text;

namespace ServiceBusQueueConsumer
{
    public class Program
    {
        private static IConfigurationRoot _configuration;
        private static string _sbConnectionString;
        private static string _sbQueueName;


        public static async Task Main(string[] args)
        {
            BuildOptions();
            Console.WriteLine("Hello World");

            //add the service bus connection and clients
            _sbConnectionString = _configuration["ServiceBus:ReadOnlyConnectionString"];
            _sbQueueName = _configuration["ServiceBus:QueueName"];

            if (string.IsNullOrEmpty(_sbConnectionString) || string.IsNullOrEmpty(_sbQueueName))
            {
                Console.WriteLine("Ensure connection string and queue name are set correctly");
                return;
            }

            await TypicalQueueProcessing();
        }

        private static async Task TypicalQueueProcessing()
        {
            var client = new ServiceBusClient(_sbConnectionString);
            ServiceBusProcessorOptions sbpo = new ServiceBusProcessorOptions();
            sbpo.ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete;

            var processor = client.CreateProcessor(_sbQueueName
                                                    , new ServiceBusProcessorOptions() { 
                                                            ReceiveMode = ServiceBusReceiveMode.PeekLock 
                                                    });
            var processorReceiveAndDelete = client.CreateProcessor(_sbQueueName, sbpo); 

            try
            {
                // add handler to process messages
                processor.ProcessMessageAsync += MessageHandler;

                // add handler to process any errors
                processor.ProcessErrorAsync += ErrorHandler;

                // start processing 
                await processor.StartProcessingAsync();

                Console.WriteLine("Wait for a minute and then press any key to end the processing");
                Console.ReadKey();

                // stop processing 
                Console.WriteLine("\nStopping the receiver...");
                await processor.StopProcessingAsync();
                Console.WriteLine("Stopped receiving messages");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                // Calling DisposeAsync on client types is required to ensure that network
                // resources and other unmanaged objects are properly cleaned up.
                await processor.DisposeAsync();
                await client.DisposeAsync();
            }
        }

        private static async Task WriteMessageToConsole(Movie movie)
        {
            Console.WriteLine($"Movie Review found: {movie.Title} was released in {movie.ReleaseYear} and is rated {movie.MPAARating}");
        }

        // handle received messages
        private static async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            Console.WriteLine($"Received: {body}");

            var movie = JsonConvert.DeserializeObject<Movie>(body);
            await WriteMessageToConsole(movie);

            // complete the message. message is deleted from the queue. 
            // This is required when using at least once to clear the message.
            // Not required when using at-most once, because it was deleted on receipt
            await args.CompleteMessageAsync(args.Message);
        }

        // handle any errors when receiving messages
        private static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine($"Error: {args.Exception.Message}");
            return Task.CompletedTask;
        }

        private static void BuildOptions()
        {
            _configuration = ConfigurationBuilderSingleton.ConfigurationRoot;
        }
    }
}
