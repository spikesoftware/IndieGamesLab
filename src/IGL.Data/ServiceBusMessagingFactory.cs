using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Configuration;

namespace IGL.Data
{
    static public class ServiceBusMessagingFactory
    {
        private readonly static string ConnectionString = ConfigurationManager.AppSettings["AzureServiceBusConnectionString"];

        private const string IGLEventHub = "igl";

        static public QueueClient GetQueueClientByName(string queueName)
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(ConnectionString);

            if (!namespaceManager.QueueExists(queueName))
            {
                namespaceManager.CreateQueue(queueName);
            }

            return QueueClient.CreateFromConnectionString(ConnectionString, queueName);
        }

        static public EventHubClient GetIGLEventHubClient()
        {
            return EventHubClient.CreateFromConnectionString(ConnectionString, IGLEventHub);
        }

        public static bool IsInitialised = VerifyConnection();

        private static bool VerifyConnection()
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(ConnectionString);

            if (!namespaceManager.EventHubExists(IGLEventHub))
            {
                namespaceManager.CreateEventHub(IGLEventHub);
            }

            return true;
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
