using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Configuration;

namespace IGL.Service
{
    static public class ServiceBusMessagingFactory
    {        
        public static string ConnectionString = ConfigurationManager.AppSettings["AzureServiceBusConnectionString"];
        
        static public QueueClient GetQueueClientByName(string queueName)
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(ConnectionString);

            if (!namespaceManager.QueueExists(queueName))
            {
                namespaceManager.CreateQueue(queueName);
            }

            return QueueClient.CreateFromConnectionString(ConnectionString, queueName);
        }

        static public TopicClient GetTopicClientByName(string topic)
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(ConnectionString);

            if (!namespaceManager.QueueExists(topic))
            {
                namespaceManager.CreateQueue(topic);
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
    }
}
