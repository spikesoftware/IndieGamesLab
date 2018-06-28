using System.Threading.Tasks;
using Microsoft.Azure.Management.ServiceBus;
using Microsoft.Azure.Management.ServiceBus.Models;
using Microsoft.Azure.ServiceBus;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using SqlFilter = Microsoft.Azure.Management.ServiceBus.Models.SqlFilter;

namespace IGL.Service
{
    public class ServiceBusMessagingFactory
    {
        public ServiceBusMessagingFactory(BackboneConfiguration configuration)
        {
            Configuration = configuration;
        }

        public BackboneConfiguration Configuration { get; set; }

        private Task<ServiceBusManagementClient> ServiceBusManagementClient
        {
            get
            {
                return Task.Run(async () =>
                {
                    var context =
                        new AuthenticationContext($"https://login.microsoftonline.com/{Configuration.TenantId}");
                    var result = await context.AcquireTokenAsync("https://management.core.windows.net/",
                        new ClientCredential(Configuration.ClientId, Configuration.ClientSecret));
                    var token = result.AccessToken;
                    var creds = new TokenCredentials(token);
                    var sbClient =
                        new ServiceBusManagementClient(creds) {SubscriptionId = Configuration.SubscriptionId};
                    return sbClient;
                });
            }
        }

        public async Task<QueueClient> GetQueueClientByName(string queueName, bool requiresSession = false)
        {
            await EnsureQueueExists(queueName, requiresSession);
            return new QueueClient(Configuration.ConnectionString, queueName);
        }

        public async Task<TopicClient> GetTopicClientByName(string topic)
        {
            await EnsureTopicExists(topic);
            return new TopicClient(Configuration.ConnectionString, topic);
        }

        public async Task CreateSubscription(string topic, string subscription, string filter)
        {
            await EnsureTopicExists(topic);
            await EnsureSubscriptionExists(topic, subscription, filter);
        }

        private async Task EnsureQueueExists(string queueName, bool requireSession = false)
        {
            var sbClient = await ServiceBusManagementClient;

            await sbClient.Queues.CreateOrUpdateAsync(Configuration.ResourceGroupName,
                Configuration.NamespaceName,
                queueName,
                new SBQueue
                {
                    RequiresSession = requireSession,
                    EnablePartitioning = true
                });
        }

        private async Task EnsureTopicExists(string topicName)
        {
            var sbClient = await ServiceBusManagementClient;

            await sbClient.Topics.CreateOrUpdateAsync(Configuration.ResourceGroupName,
                Configuration.NamespaceName,
                topicName,
                new SBTopic
                {
                    EnablePartitioning = true
                });
        }

        private async Task EnsureSubscriptionExists(string topicName, string subscriptionName, string filter)
        {
            var sbClient = await ServiceBusManagementClient;

            await sbClient.Subscriptions.CreateOrUpdateAsync(Configuration.ResourceGroupName,
                Configuration.NamespaceName,
                topicName,
                subscriptionName,
                new SBSubscription());

            await sbClient.Rules.CreateOrUpdateAsync(Configuration.ResourceGroupName,
                Configuration.NamespaceName,
                topicName,
                subscriptionName,
                "Default",
                new Rule
                {
                    SqlFilter = new SqlFilter(filter)
                });
        }
    }
}