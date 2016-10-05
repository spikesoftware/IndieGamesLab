#if !DO_NOT_FAKE
using IGL.Data.ServiceEntities;
using IGL.Service.Common;
using Microsoft.ServiceBus.Fakes;
using Microsoft.ServiceBus.Messaging;
using Microsoft.ServiceBus.Messaging.Fakes;
using Microsoft.WindowsAzure.Storage.Fakes;
using Microsoft.WindowsAzure.Storage.Table.Fakes;
using System;
using System.Collections.Generic;
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
                                var zz = new ShimCloudTable()
                                {
                                    NameGet = () => "faked",
                                    CreateIfNotExistsTableRequestOptionsOperationContext = (x, y) => true,
                                                                       ExecuteQueryTableQueryTableRequestOptionsOperationContext = (query, z, k) =>
                                    {
                                        var entity1 = new ShimDynamicTableEntity().Bind(new RoleTaskDefinitionEntity { PartitionKey = "P1", RowKey = "R1", Name = "Name", Type = "Type", QueueName = "QueueName" });
                                        var entity2 = new ShimDynamicTableEntity().Bind(new RoleTaskDefinitionEntity { PartitionKey = "P2", RowKey = "R2", Name = "Name", Type = "Type", QueueName = "QueueName" });

                                        var result = new List<Microsoft.WindowsAzure.Storage.Table.DynamicTableEntity>();
                                        result.Add(entity1);
                                        result.Add(entity2);

                                        return result;
                                    },
                                    ExecuteTableOperationTableRequestOptionsOperationContext = (operation, z, k) =>
                                    {
                                        return new ShimTableResult();
                                    }                                    
                                };

                                zz.ExecuteQueryOf1TableQueryOfM0TableRequestOptionsOperationContext<RoleTaskDefinitionEntity>((query, options, context) =>
                                {
                                    var entity1 = new RoleTaskDefinitionEntity { PartitionKey = "P1", RowKey = "R1", Name = "Name", Type = "Type", QueueName = "QueueName" };
                                    var entity2 = new RoleTaskDefinitionEntity { PartitionKey = "P2", RowKey = "R2", Name = "Name", Type = "Type", QueueName = "QueueName" };

                                    var result = new List<RoleTaskDefinitionEntity>();
                                    result.Add(entity1);
                                    result.Add(entity2);

                                    return result;
                                });

                                return zz;
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