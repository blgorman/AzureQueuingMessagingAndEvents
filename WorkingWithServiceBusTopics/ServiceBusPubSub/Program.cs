using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Configuration;
using MySolutionObjectModels;
using Newtonsoft.Json;
using System.Text;

//Code samples and ideas can be researched here:
//https://docs.microsoft.com/azure/service-bus-messaging/service-bus-dotnet-how-to-use-topics-subscriptions?WT.mc_id=AZ-MVP-5004334
//https://docs.microsoft.com/azure/service-bus-messaging/service-bus-filter-examples?WT.mc_id=AZ-MVP-5004334

namespace ServiceBusPubSub
{
    public class Program
    {
        private static IConfigurationRoot _configuration;

        // connection string to the Service Bus namespace
        private static string _sbConnectionString = string.Empty;

        // name of the Service Bus topic
        private static string _sbTopicName = string.Empty;

        // the client that owns the connection and can be used to create senders and receivers
        private static ServiceBusClient _sbClient;

        // the sender used to publish messages to the topic
        private static ServiceBusSender _sbSender;

        public static async Task Main()
        {
            BuildOptions();
            Console.WriteLine("Service Bus Pub/Sub producer started");

            _sbConnectionString = _configuration["ServiceBus:WriteOnlySASConnectionString"];
            _sbTopicName = _configuration["ServiceBus:TopicName"];

            _sbClient = new ServiceBusClient(_sbConnectionString);
            _sbSender = _sbClient.CreateSender(_sbTopicName);

            await SendMessagesToTopicAsync(_sbSender);

            //note: this will print async out of order so you'll just need to hit enter to kill the program
            Console.WriteLine("All movies added to the topic");
            Console.WriteLine("Press any key to end the application");
            Console.ReadKey();
        }

        private static void BuildOptions()
        {
            _configuration = ConfigurationBuilderSingleton.ConfigurationRoot;
        }

        private static async Task SendMessagesToTopicAsync(ServiceBusSender sender)
        {
            
            Console.WriteLine("Sending all movies to the topic...");

            var movies = GetMovies();

            //Send each movie
            movies.ForEach(async x => await SendMovie(sender, x));

            Console.WriteLine("All messages sent.");
        }

        private static async Task SendMovie(ServiceBusSender sender, Movie movie)
        {
            var message = new ServiceBusMessage(
                                Encoding.UTF8.GetBytes(
                                    JsonConvert.SerializeObject(movie)))
            {
                CorrelationId = movie.Id,
                Subject = movie.MPAARating,
                ApplicationProperties =
                {
                    { "Id", movie.Id },
                    { "Title", movie.Title },
                    { "MPAARating", movie.MPAARating },
                    { "ReleaseYear", movie.ReleaseYear }
                }
            };
            await sender.SendMessageAsync(message);

            Console.WriteLine($"Sent movie to topic with id: {movie.Id}, Title: {movie.Title}, MPAARating: {movie.MPAARating}, ReleaseYear: {movie.ReleaseYear}");
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
