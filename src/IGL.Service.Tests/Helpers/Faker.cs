#if !DO_NOT_FAKE
using Microsoft.ServiceBus.Fakes;
using Microsoft.ServiceBus.Messaging;
using Microsoft.ServiceBus.Messaging.Fakes;
using Microsoft.WindowsAzure.Storage.Fakes;
using Microsoft.WindowsAzure.Storage.Table.Fakes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;

namespace IGL.Service.Tests.Helpers
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

            var goodMessage = new ShimBrokeredMessage()
            {
                CompleteAsync = () => null                
            };

            ShimBrokeredMessage.AllInstances.GetBodyOf1XmlObjectSerializer<GamePacket>((x, ser) => new GamePacket() { EventId = 2 });

            ShimQueueClient.CreateFromConnectionStringStringString = (connectionstring, table) =>
            {
                return new ShimQueueClient(queueClient)
                {
                    ReceiveBatchAsyncInt32TimeSpan = (size, waitTime) =>
                    {
                        var msgs = new List<BrokeredMessage>();

                        switch (m)
                        {
                            case 0:
                                m++;

                                // return 10 good messages
                                for (int i = 0; i < 10; i++)
                                    msgs.Add(goodMessage);

                                return Task.FromResult<System.Collections.Generic.IEnumerable<BrokeredMessage>>(msgs);
                            case 1:
                                m++;

                                // return 20 good messages and 1 bad
                                for (int i = 0; i < 20; i++)
                                    msgs.Add(goodMessage);

                                msgs.Add(null);

                                return Task.FromResult<System.Collections.Generic.IEnumerable<BrokeredMessage>>(msgs);                                
                            case 2: // no activity
                            case 3: // no activity
                            case 4: // no activity
                            default:
                                m++;
                                Thread.Sleep(30);
                                return Task.FromResult<System.Collections.Generic.IEnumerable<BrokeredMessage>>(new List<BrokeredMessage>());
                            case 5:
                                m++;

                                // return 20 good messages and 5 bad
                                msgs.Add(null);
                                msgs.Add(null);
                                msgs.Add(null);

                                for (int i = 0; i < 20; i++)
                                    msgs.Add(goodMessage);

                                msgs.Add(null);
                                msgs.Add(null);                                

                                return Task.FromResult<System.Collections.Generic.IEnumerable<BrokeredMessage>>(msgs);
                        }
                    }                    
                };
            };
        }

        static int m = 0;
    }
}
#endif