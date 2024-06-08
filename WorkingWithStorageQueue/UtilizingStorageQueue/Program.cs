using Microsoft.Extensions.Configuration;
using Azure;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System;
using System.Threading.Tasks;
using MySolutionObjectModels;
using Newtonsoft.Json;

namespace UtilizingStorageQueue
{
    //https://docs.microsoft.com/en-us/azure/storage/queues/storage-quickstart-queues-dotnet?tabs=environment-variable-windows
    /// <summary>
    /// Automatically creates the queue if it doesn't exist.  Also shows adding, processing, and removing messages from the queue
    /// </summary>
    public class Program
    {
        private static IConfigurationRoot _configuration;

        private static string _sqConnectionString;
        private static string _sqName;
        private static QueueClient _sqClient;

        public static async Task Main(string[] args)
        {
            BuildOptions();
            Console.WriteLine("Hello Storage Queue");

            _sqConnectionString = _configuration["StorageAccount:ConnectionString"];
            _sqName = _configuration["StorageAccount:QueueName"];

            // Instantiate a QueueClient which will be
            // used to create and manipulate the queue
            _sqClient = new QueueClient(_sqConnectionString, _sqName);

            // Create the queue
            await _sqClient.CreateIfNotExistsAsync();

            Console.WriteLine("\nAdding messages to the queue...");

            // Send several messages to the queue
            var movies = GetMovies();
            var receipts = new List<SendReceipt>();

            foreach (var movie in movies)
            {
                receipts.Add(await _sqClient.SendMessageAsync(JsonConvert.SerializeObject(movie)));
            }

            //peek
            Console.WriteLine("\nPeek at the messages in the queue...");

            // Peek at messages in the queue
            PeekedMessage[] peekedMessages = await _sqClient.PeekMessagesAsync(maxMessages: 10);

            foreach (PeekedMessage peekedMessage in peekedMessages)
            {
                // Display the message
                Console.WriteLine($"MessageId: {peekedMessage.MessageId} | Message: {peekedMessage.MessageText}");
            }

            //update
            Console.WriteLine("\nUpdating the first message in the queue...");

            // Update a message using the saved receipt from sending the message
            var newMovie = new Movie() { Id = "235234QAW", MPAARating = "PG-13"
                                        , Title = "Top Gun: Maverick", ReleaseYear = 2022 };
            await _sqClient.UpdateMessageAsync(receipts[0].MessageId
                                                , receipts[0].PopReceipt
                                                , JsonConvert.SerializeObject(newMovie));
            

            //receive [but leave]
            Console.WriteLine("\nReceiving messages from the queue...");

            // Get messages from the queue
            var messages = await _sqClient.ReceiveMessagesAsync(maxMessages: 20);

            foreach (var m in messages.Value)
            {
                var qMovie = JsonConvert.DeserializeObject<Movie>(m.Body.ToString());
                Console.WriteLine($"Received Message deserialized to Movie {qMovie.Title}");
            }

            //delete messages
            Console.WriteLine("\nPress Enter key to 'process' messages (deletes the previously read above from the queue)");
            Console.ReadLine();

            //lease extension[do not do this if you are going to delete messages]
            Console.WriteLine("Would you like to lease the mesages for one hour (NOTE: you will NOT able to delete message for one hour if you do this!)?");
            var leaseMessages = Console.ReadLine().ToLower().StartsWith('y');
            if (leaseMessages)
            {
                TimeSpan ts = new TimeSpan(1);
                foreach (QueueMessage message in messages.Value)
                {
                    Console.WriteLine($"Renewing Lease for 1 hour on {message.MessageId}");
                    await _sqClient.UpdateMessageAsync(message.MessageId
                                                        , message.PopReceipt
                                                        , message.Body, ts);
                }

                foreach (QueueMessage message in messages.Value)
                {
                    Console.WriteLine($"Message: {message.MessageId} " +
                                        $"Next visible on: {message.NextVisibleOn}");
                }
            }
                

            // Process and delete messages from the queue
            foreach (QueueMessage message in messages.Value)
            {
                // "Process" the message
                Console.WriteLine($"Message: {message.MessageText} processed" +
                                        $", now deleting...");

                // Let the service know we're finished with
                // the message and it can be safely deleted.
                await _sqClient.DeleteMessageAsync(message.MessageId
                                                    , message.PopReceipt);
            }

            Console.WriteLine("\nMessages are processed. Press Enter key to continue...");
            Console.ReadLine();

            // Clean up
            Console.WriteLine("Would you like to delete the queue?");
            var deleteIt = Console.ReadLine().ToLower().StartsWith('y');
            if (deleteIt)
            {
                Console.WriteLine($"Deleting queue: {_sqClient.Name}");
                await _sqClient.DeleteAsync();
            }
            else
            {
                Console.WriteLine("Queue not deleted");
            }

            Console.WriteLine("Done");
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

        private static void BuildOptions()
        {
            _configuration = ConfigurationBuilderSingleton.ConfigurationRoot;
        }
    }
}
