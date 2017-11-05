using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Configuration;

namespace IGL.Service
{
    static public class ServiceBusMessagingFactory
    {        
        public static string ConnectionString = ConfigurationManager.AppSettings["AzureServiceBusConnectionString"];
        
        static public QueueClient GetQueueClientByName(string queueName, bool requiresSession = false)
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(ConnectionString);

            if (!namespaceManager.QueueExists(queueName))
            {
                var description = new QueueDescription(queueName)
                {
                    RequiresSession = requiresSession
                };

                namespaceManager.CreateQueue(description);
            }

            return QueueClient.CreateFromConnectionString(ConnectionString, queueName);
        }

        static public TopicClient GetTopicClientByName(string topic)
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(ConnectionString);

            if (!namespaceManager.TopicExists(topic))
            {
                namespaceManager.CreateTopic(topic);
            }

            return TopicClient.CreateFromConnectionString(ConnectionString, topic);
        }

        public static long GetQueueSize(string queueName)
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(ConnectionString);

            if (namespaceManager.QueueExists(queueName))
            {
                return namespaceManager.GetQueue(queueName).MessageCountDetails.ActiveMessageCount;
            }

            throw new ApplicationException(string.Format("Queue {0} has not been initialised.  Use ServiceBusMessagingFactory.Initialise() first.", queueName));
        }

        public static void CreateSubscription(string topic, string subscription)
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(ConnectionString);
            
            if (namespaceManager.TopicExists(topic) && !namespaceManager.SubscriptionExists(topic, subscription))
            {
                namespaceManager.CreateSubscription(topic, subscription);
            }
        }
    }
}
