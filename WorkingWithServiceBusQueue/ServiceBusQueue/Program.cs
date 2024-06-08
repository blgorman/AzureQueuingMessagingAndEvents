using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using MySolutionObjectModels;
using Newtonsoft.Json;
using System.Diagnostics;

namespace ServiceBusQueue
{
    public class Program
    {
        private static IConfigurationRoot _configuration;


        public static async Task Main(string[] args)
        {
            BuildOptions();
            Console.WriteLine("Service Bus Queue Publisher");

            var theMovies = GetMovies();

            //add the service bus connection and clients
            var sbConnectionString = _configuration["ServiceBus:WriteOnlyConnectionString"]; 
            var sbQueueName = _configuration["ServiceBus:QueueName"]; 
            var client = new ServiceBusClient(sbConnectionString);
            var sender = client.CreateSender(sbQueueName);

            // create a batch
            using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();
            {
                try
                {
                    foreach (var m in theMovies)
                    {
                        var moviesJSON = JsonConvert.SerializeObject(m); 

                        if (!messageBatch.TryAddMessage(new ServiceBusMessage(moviesJSON)))
                        {
                            // if an exception occurs
                            throw new Exception($"Exception has occurred adding message {moviesJSON} to batch.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    //consider logging something more verbose to application insights
                    Console.WriteLine($"{ex.Message}");
                }

                try
                {
                    await sender.SendMessagesAsync(messageBatch);
                    Console.WriteLine($"Batch processed {messageBatch.Count} messages " +
                                            $"to the queue for movie review list");
                }
                catch (Exception ex)
                {
                    //consider logging something more verbose to application insights
                    Console.WriteLine($"Exception sending messages as batch: {ex.Message}");
                }
                finally
                {
                    //clean up resources
                    await sender.DisposeAsync();
                    await client.DisposeAsync();
                }
            }
        }

        private static void BuildOptions()
        {
            _configuration = ConfigurationBuilderSingleton.ConfigurationRoot;
        }

        private static List<Movie> GetMovies()
        { 
            return new List<Movie>() { 
                new Movie() { Id = "6305127689", Title = "Top Gun", MPAARating = "PG", ReleaseYear = 1986 },
                new Movie() { Id = "B099WQYXLD", Title = "The Shawshank Redemption", MPAARating = "R", ReleaseYear = 1994 },
                new Movie() { Id = "B06XNPG2XF", Title = "The Godfather", MPAARating = "R", ReleaseYear = 1972 },
                new Movie() { Id = "1419861190", Title = "The Dark Knight", MPAARating = "PG-13", ReleaseYear = 2008 },
                new Movie() { Id = "B06XNRW1VQ", Title = "The Godfather: Part II", MPAARating = "R", ReleaseYear = 1974 },
                new Movie() { Id = "B0010YSD7W", Title = "12 Angry Men", MPAARating = "NR", ReleaseYear = 1957 },
                new Movie() { Id = "B07JJ5WH62", Title = "Schindler's List", MPAARating = "R", ReleaseYear = 1993 },
                new Movie() { Id = "B000634DCW", Title = "The Lord of the Rings: The Return of the King", MPAARating = "PG-13", ReleaseYear = 2003 },
                new Movie() { Id = "B00412MU1A", Title = "Pulp Fiction", MPAARating = "PG", ReleaseYear = 1994 },
                new Movie() { Id = "B009NYCH6E", Title = "The Lord of the Rings: The Fellowship of the Ring", MPAARating = "PG-13", ReleaseYear = 2001 },
                new Movie() { Id = "B07MS59ZL6", Title = "The Good, the Bad and the Ugly", MPAARating = "R", ReleaseYear = 1966 },
                new Movie() { Id = "B01MFASCJO", Title = "Forrest Gump", MPAARating = "PG", ReleaseYear = 1994 },
                new Movie() { Id = "B0007DFJ0G", Title = "Fight Club", MPAARating = "R", ReleaseYear = 1999 },
                new Movie() { Id = "B00UGPJ6RC", Title = "Inception ", MPAARating = "PG-13", ReleaseYear = 2010 },
                new Movie() { Id = "B00009TB5G", Title = "The Lord of the Rings: The Two Towers", MPAARating = "PG-13", ReleaseYear = 2002 },
                new Movie() { Id = "B07TNVX3L7", Title = "Star Wars: Episode V - The Empire Strikes Back", MPAARating = "PG", ReleaseYear = 1980 },
                new Movie() { Id = "B000P0J0AQ", Title = "The Matrix", MPAARating = "R", ReleaseYear = 1999 }
            };
        
        }
    }
}
