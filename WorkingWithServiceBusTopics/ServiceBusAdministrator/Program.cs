using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Configuration;

namespace ServiceBusAdministrator
{
    /// <summary>
    /// Note: This code is idempotent so you can run multiple times without error, even if topics/subscriptions do not or already exist.
    /// </summary>
    public class Program
    {
        private static IConfigurationRoot _configuration;
        // Service Bus Administration Client object to create topics and subscriptions
        private static ServiceBusAdministrationClient adminClient;

        // connection string to the Service Bus namespace
        private static string _sbConnectionString = string.Empty;

        // name of the Service Bus topic
        private static string _sbTopicName = string.Empty;

        // names of subscriptions to the topic
        private static string _sbSubscriptionAllMovies = string.Empty;
        private static string _sbSubscriptionAdultMovies = string.Empty;
        private static string _sbSubscriptionFamilyMovies = string.Empty;

        public static async Task Main(string[] args)
        {
            BuildOptions();
            _sbConnectionString = _configuration["ServiceBus:NamespaceRootAccessConnectionString"];
            _sbTopicName = _configuration["ServiceBus:TopicName"];
            _sbSubscriptionFamilyMovies = _configuration["ServiceBus:SubscriptionNameFamily"];
            _sbSubscriptionAdultMovies = _configuration["ServiceBus:SubscriptionNameAdult"];
            _sbSubscriptionAllMovies = _configuration["ServiceBus:SubscriptionAll"];

            try
            {
                Console.WriteLine("Creating the Service Bus Administration Client object");
                adminClient = new ServiceBusAdministrationClient(_sbConnectionString);

                bool topicExists = false;
                try
                {
                    var existingTopic = await adminClient.GetTopicAsync(_sbTopicName);
                    topicExists = existingTopic != null;
                    Console.WriteLine($"Topic {_sbTopicName} exists");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not find existing topic {_sbTopicName}");
                }
                
                if (!topicExists)
                {
                    Console.WriteLine($"Creating the topic {_sbTopicName}");
                    await adminClient.CreateTopicAsync(_sbTopicName);
                }

                bool subscriptionExists = false;
                try
                {
                    var anExistingSubscription = 
                        await adminClient.GetSubscriptionAsync(_sbTopicName, _sbSubscriptionAllMovies);
                    subscriptionExists = anExistingSubscription is not null;
                    Console.WriteLine($"Subscription {_sbSubscriptionAllMovies} exists");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not find existing subscription {_sbSubscriptionAllMovies}");
                }

                if (!subscriptionExists)
                {
                    Console.WriteLine($"Creating the subscription {_sbSubscriptionAllMovies} for the topic with a True filter ");
                    // Create a True Rule filter with an expression that always evaluates to true
                    // It's equivalent to using SQL rule filter with 1=1 as the expression
                    await adminClient.CreateSubscriptionAsync(
                            new CreateSubscriptionOptions(_sbTopicName, _sbSubscriptionAllMovies),
                            new CreateRuleOptions("AllMovies", new TrueRuleFilter()));
                }

                subscriptionExists = false;
                try
                {
                    var anExistingSubscription = await adminClient.GetSubscriptionAsync(_sbTopicName, _sbSubscriptionAdultMovies);
                    subscriptionExists = anExistingSubscription is not null;
                    Console.WriteLine($"Subscription {_sbSubscriptionAdultMovies} exists");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not find existing subscription {_sbSubscriptionAdultMovies}");
                }
                
                if (!subscriptionExists)
                {
                    Console.WriteLine($"Creating the subscription {_sbSubscriptionAdultMovies} with a SQL filter");
                    // Create a SQL filter with MPAARating set to "PG-13", or "R"
                    await adminClient.CreateSubscriptionAsync(
                            new CreateSubscriptionOptions(_sbTopicName, _sbSubscriptionAdultMovies),
                            new CreateRuleOptions("AllAdultMovies"
                                            , new SqlRuleFilter("MPAARating='PG-13' OR MPAARating = 'R'")));
                }


                subscriptionExists = false;
                try
                {
                    var anExistingSubscription = 
                        await adminClient.GetSubscriptionAsync(_sbTopicName, _sbSubscriptionFamilyMovies);
                    subscriptionExists = anExistingSubscription is not null;
                    Console.WriteLine($"Subscription {_sbSubscriptionFamilyMovies} exists");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not find existing subscription {_sbSubscriptionFamilyMovies}");
                }

                if (!subscriptionExists)
                {
                    Console.WriteLine($"Creating the subscription {_sbSubscriptionFamilyMovies} with a SQL filter");
                    // Create a SQL filter with MPAARating set to "PG-13", or "R"
                    await adminClient.CreateSubscriptionAsync(
                        new CreateSubscriptionOptions(_sbTopicName, _sbSubscriptionFamilyMovies),
                        new CreateRuleOptions("AllFamilyMovies", new SqlRuleFilter("MPAARating='PG' OR MPAARating = 'G'")));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.WriteLine("Topic and subscriptions exist as required");  

        }

        private static void BuildOptions()
        {
            _configuration = ConfigurationBuilderSingleton.ConfigurationRoot;
        }
    }
}
