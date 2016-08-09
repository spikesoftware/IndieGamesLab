#if !FAKES_NOT_SUPPORTED
using Microsoft.ServiceBus.Fakes;
using Microsoft.ServiceBus.Messaging;
using Microsoft.ServiceBus.Messaging.Fakes;
using Microsoft.WindowsAzure.Storage.Fakes;
using Microsoft.WindowsAzure.Storage.Table.Fakes;
using System;
using System.Collections.Specialized;


namespace IGL.Data.Test.Helpers
{
    public class Faker
    {
        public static void FakeOut()
        {
            var collection = new NameValueCollection();
            collection.Add("AzureStorageConnectionString", "AzureStorageConnectionString");
            System.Configuration.Fakes.ShimConfigurationManager.AppSettingsGet = () => collection;

            ShimCloudStorageAccount.ParseString = (s) =>
            {
                if (s == "AzureStorageConnectionString")
                    return new ShimCloudStorageAccount
                    {
                        CreateCloudTableClient = () => new ShimCloudTableClient
                        {
                            GetTableReferenceString = (tableReference) =>
                            {
                                return new ShimCloudTable
                                {
                                    CreateIfNotExistsTableRequestOptionsOperationContext = (x, y) => true,
                                    ExecuteTableOperationTableRequestOptionsOperationContext = (operation, z, k) =>
                                    {
                                        return new ShimTableResult();
                                    }
                                };
                            }
                        }
                    };

                throw new Exception("AzureStorageConnectionString was not set correctly!");
            };

            ShimNamespaceManager.CreateFromConnectionStringString = (connectionString) =>
            {
                return new ShimNamespaceManager()
                {
                    QueueExistsString = (queue) => true,
                    EventHubExistsString = (hub) => true,
                    GetQueueString = (queue) => new ShimQueueDescription
                    {
                        MessageCountDetailsGet = () => new ShimMessageCountDetails
                        {
                            ActiveMessageCountGet = () => 10
                        }
                    }
                };
            };

            var queueClient = QueueClient.CreateFromConnectionString("Endpoint=sb://fake.net/;", "GameEvents");

            ShimQueueClient.CreateFromConnectionStringStringString = (connectionstring, table) =>
            {
                return new ShimQueueClient(queueClient);
            };
        }
    }
}
#endif