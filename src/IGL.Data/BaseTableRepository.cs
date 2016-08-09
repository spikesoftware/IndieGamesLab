using IGL.Data.ServiceEntities;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;

namespace IGL.Data
{
    public abstract class BaseTableRepository<T> where T : BaseTableEntity, new()
    {
        private readonly string ConnectionString = ConfigurationManager.AppSettings["AzureStorageConnectionString"];
        private const string GameEventsTable = "GameEvents";

        protected readonly string _tableName;
        protected readonly CloudTable _table;
        protected readonly int _id;
        protected TraceSource Trace = new TraceSource("IndieGamesLab");

        public BaseTableRepository(string tableName, int id)
        {
            this._tableName = tableName;
            this._table = GetClientForTable();
            this._id = id;
        }

        protected AzureResult InsertOrReplace(T item)
        {
            var sw = Stopwatch.StartNew();

            try
            {
                item.CreatedDate = DateTime.Now;
                item.LastUpdatedDate = DateTime.Now;

                _table.Execute(TableOperation.InsertOrReplace(item));                
            }
            catch (Exception ex)
            {
                Trace.TraceEvent(TraceEventType.Error, GetEventId(RepositoryAction.Add),
                                "BaseTableRepository.InsertOrReplace(Table:{0} PartitionKey:{1} RowKey{2}) failed with:{3}\nInnerMessage:{4}\nStackTrace:{5}",
                                this._tableName,
                                item.PartitionKey,
                                item.RowKey,
                                ex.Message,
                                ex.InnerException == null ? "" : ex.InnerException.Message,
                                ex.StackTrace);

                throw ex;
            }
            finally
            {
                sw.Stop();
                Trace.TraceInformation("BaseTableRepository.InsertOrReplace(Table:{0} PartitionKey:{1} RowKey{2}) completed in {3} ticks.", this._tableName, item.PartitionKey, item.RowKey, sw.ElapsedTicks);
            }

            return new AzureResult
            {
                Code = 0
            };
        }

        protected AzureResult Delete(T item)
        {
            var sw = Stopwatch.StartNew();

            try
            {
                _table.Execute(TableOperation.Delete(item));                
            }
            catch (Exception ex)
            {
                Trace.TraceEvent(TraceEventType.Error, GetEventId(RepositoryAction.Delete),
                                "BaseTableRepository.Delete(Table:{0} PartitionKey:{1} RowKey{2}) failed with:{3}\nInnerMessage:{4}\nStackTrace:{5}",
                                this._tableName,
                                item.PartitionKey,
                                item.RowKey,
                                ex.Message,
                                ex.InnerException == null ? "" : ex.InnerException.Message,
                                ex.StackTrace);

                throw ex;
            }
            finally
            {
                sw.Stop();
                Trace.TraceInformation("BaseTableRepository.Delete(Table:{0} PartitionKey:{1} RowKey{2}) completed in {3} ticks.", this._tableName, item.PartitionKey, item.RowKey, sw.ElapsedTicks);
            }            

            return new AzureResult
            {
                Code = 0
            };
        }

        private CloudTable GetClientForTable()
        {
            var account = CloudStorageAccount.Parse(ConnectionString);

            var tableClient = account.CreateCloudTableClient();
            var table = tableClient.GetTableReference(_tableName);

            table.CreateIfNotExists();
           
            return table;
        }

        protected List<T> Get(string partitionKey)
        {
            var sw = Stopwatch.StartNew();

            try
            {
                var query = new TableQuery<T>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));

                return _table.ExecuteQuery(query).ToList();
            }
            catch (Exception ex)
            {
                Trace.TraceEvent(TraceEventType.Error, GetEventId(RepositoryAction.Get),
                                "AzureTableRepository.Get(Table:{0} PartitionKey:{1}) failed with:{3}\nInnerMessage:{4}\nStackTrace:{5}",
                                this._tableName,
                                partitionKey,
                                ex.Message,
                                ex.InnerException == null ? "" : ex.InnerException.Message,
                                ex.StackTrace);

                throw ex;
            }
            finally
            {
                sw.Stop();
                Trace.TraceInformation("AzureTableRepository.Get(Table:{0} PartitionKey:{1}) completed in {2} ticks.", this._tableName, partitionKey, sw.ElapsedTicks);
            }
        }
        protected T Get(string partitionKey, string rowKey)
        {
            var sw = Stopwatch.StartNew();

            try
            {
                return _table.Execute(TableOperation.Retrieve(partitionKey, rowKey)).Result as T;                
            }
            catch (Exception ex)
            {
                Trace.TraceEvent(TraceEventType.Error, GetEventId(RepositoryAction.Get),
                                "AzureTableRepository.Get(Table:{0} PartitionKey:{1} RowKey{2}) failed with:{3}\nInnerMessage:{4}\nStackTrace:{5}",
                                this._tableName,
                                partitionKey,
                                rowKey,
                                ex.Message,
                                ex.InnerException == null ? "" : ex.InnerException.Message,
                                ex.StackTrace);

                throw ex;
            }
            finally
            {
                sw.Stop();
                Trace.TraceInformation("AzureTableRepository.Get(Table:{0} PartitionKey:{1} RowKey{2}) completed in {3} ticks.", this._tableName, partitionKey, rowKey, sw.ElapsedTicks);
            }
        }

        enum RepositoryAction { Add = 100, Delete = 200, Update = 300, Get = 0 };
        private int GetEventId(RepositoryAction action)
        {
            return _id + (int)action;
        }
    }

}
