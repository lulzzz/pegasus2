using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAE.SMS
{
    public class StorageManager
    {

        public StorageManager()
        {
            storageAccountName = ConfigurationManager.AppSettings["storageAccountName"];
            storageKey = ConfigurationManager.AppSettings["storageKey"];
            userTableName = ConfigurationManager.AppSettings["userTableName"];
            phoneTableName = ConfigurationManager.AppSettings["phoneTableName"];
            phoneMessageTableName = ConfigurationManager.AppSettings["phoneMessageTableName"];
            userMessageTableName = ConfigurationManager.AppSettings["userMessageTableName"];
            storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=pegasus2;AccountKey=M7Fh6ZzuL626vQ5YC2wcLMCBnEC3NmgXyoBuZjfwfrqCzFykJknKxfHmHxFE15br1/XQIJMkIVtGxwIwcDvoPw==");

            StorageCredentials creds = new StorageCredentials(storageAccountName, storageKey);
            client = new CloudTableClient(new Uri(String.Format("https://{0}.table.core.windows.net/", storageAccountName)), creds);
        }


        private CloudStorageAccount storageAccount;
        private string storageAccountName;
        private string storageKey;
        private string userTableName;
        private string phoneTableName;
        private CloudTableClient client;
        private string phoneMessageTableName;
        private string userMessageTableName;

        //public async Task WriteAsync(string email, int code, string id)
        //{
        //    //CloudTableClient client = new CloudTableClient(storageAccount.TableEndpoint, storageAccount.Credentials);

        //    Retry.ExecuteAsync(async () =>
        //    {
        //        CloudTable table = client.GetTableReference(userTableName);
        //        await table.CreateIfNotExistsAsync();

        //        UserEntity entity = new UserEntity(id, code, email);

        //        TableOperation insertOperation = TableOperation.Insert(entity);
        //        await table.ExecuteAsync(insertOperation);
        //    }, TimeSpan.FromSeconds(2), 3);

        //}

        //public void WriteMessage(string phoneNumber, string message)
        //{
        //    //CloudTableClient client = new CloudTableClient(storageAccount.TableEndpoint, storageAccount.Credentials);

        //    Retry.Execute(() =>
        //    {
        //        CloudTable table = client.GetTableReference(phoneMessageTableName);
        //        table.CreateIfNotExists();

        //        PhoneMessage entity = new PhoneMessage(phoneNumber, message);
        //        entity.PartitionKey = Guid.NewGuid().ToString();
        //        entity.RowKey = phoneNumber;

        //        TableOperation insertOperation = TableOperation.Insert(entity);
        //        table.Execute(insertOperation);

        //    }, TimeSpan.FromSeconds(2), 3);


        //}

        //public void WriteUserMessage(UserMessageEntity entity)
        //{
        //    //CloudTableClient client = new CloudTableClient(storageAccount.TableEndpoint, storageAccount.Credentials);

        //    Retry.Execute(() =>
        //    {
        //        CloudTable table = client.GetTableReference(userMessageTableName);
        //        table.CreateIfNotExists();

        //        //PhoneMessage entity = new PhoneMessage(phoneNumber, message);
        //        //entity.PartitionKey = Guid.NewGuid().ToString();
        //        //entity.RowKey = phoneNumber;

        //        TableOperation insertOperation = TableOperation.Insert(entity);
        //        table.Execute(insertOperation);

        //    }, TimeSpan.FromSeconds(2), 3);


        //}

        //public async Task Write(string email, int code, string id)
        //{
        //    //CloudTableClient client = new CloudTableClient(storageAccount.TableEndpoint, storageAccount.Credentials);

        //    Retry.ExecuteAsync(() =>
        //    {
        //        CloudTable table = client.GetTableReference(userTableName);
        //        table.CreateIfNotExists();

        //        UserEntity entity = new UserEntity(id, code, email);

        //        TableOperation insertOperation = TableOperation.Insert(entity);
        //        table.Execute(insertOperation);

        //    }, TimeSpan.FromSeconds(2), 3);

        //}


        public async Task AddAsync(PhoneEntity phone)
        {
            //CloudTableClient client = new CloudTableClient(storageAccount.TableEndpoint, storageAccount.Credentials);
            //client.DefaultRequestOptions = new TableRequestOptions() { RetryPolicy = new Microsoft.WindowsAzure.Storage.RetryPolicies.ExponentialRetry(TimeSpan.FromSeconds(3), 5) };


            CloudTable table = client.GetTableReference(phoneTableName);
            await table.CreateIfNotExistsAsync();
            Trace.TraceWarning("table exists called");

            TableOperation insertOperation = TableOperation.Insert(phone);
            await table.ExecuteAsync(insertOperation);
            Trace.TraceWarning("table insert called");

            //Retry.ExecuteAsync(async () =>
            //{
            //    CloudTable table = client.GetTableReference(phoneTableName);
            //    await table.CreateIfNotExistsAsync();
            //    TableOperation insertOperation = TableOperation.Insert(phone);
            //    await table.ExecuteAsync(insertOperation);

            //}, TimeSpan.FromSeconds(2), 3);
        }

        public void Add(PhoneEntity phone)
        {
            try
            {
                //CloudTableClient client = new CloudTableClient(storageAccount.TableEndpoint, storageAccount.Credentials);


                //client.DefaultRequestOptions = new TableRequestOptions() { RetryPolicy = new Microsoft.WindowsAzure.Storage.RetryPolicies.ExponentialRetry(TimeSpan.FromSeconds(3), 5) };

                CloudTable table = client.GetTableReference(phoneTableName);


                table.CreateIfNotExists();
                TableOperation insertOperation = TableOperation.Insert(phone);
                table.Execute(insertOperation);

            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                throw ex;
            }
            //}, TimeSpan.FromSeconds(2), 3);

        }


        //public async Task AddAsync(UserEntity user)
        //{
        //    //CloudTableClient client = new CloudTableClient(storageAccount.TableEndpoint, storageAccount.Credentials);


        //    Retry.ExecuteAsync(async () =>
        //    {
        //        CloudTable table = client.GetTableReference(userTableName);
        //        await table.CreateIfNotExistsAsync();

        //        TableOperation insertOperation = TableOperation.Insert(user);
        //        await table.ExecuteAsync(insertOperation);

        //    }, TimeSpan.FromSeconds(2), 3);
        //}

        public async Task WriteAsync(string phone, bool activated)
        {
            //CloudTableClient client = new CloudTableClient(storageAccount.TableEndpoint, storageAccount.Credentials);

            Retry.ExecuteAsync(async () =>
            {
                CloudTable table = client.GetTableReference(phoneTableName);
                await table.CreateIfNotExistsAsync();

                PhoneEntity entity = new PhoneEntity(phone, activated);

                TableOperation insertOperation = TableOperation.Insert(entity);
                await table.ExecuteAsync(insertOperation);

            }, TimeSpan.FromSeconds(2), 3);


        }

        //public List<UserEntity> GetUsers()
        //{
        //    //CloudTableClient client = new CloudTableClient(storageAccount.TableEndpoint, storageAccount.Credentials);

        //    CloudTable table = client.GetTableReference(userTableName);
        //    var query = new TableQuery<UserEntity>();

        //    TableQuerySegment<UserEntity> segment = table.ExecuteQuerySegmented<UserEntity>(query, new TableContinuationToken());

        //    if (!(segment == null || segment.Results.Count == 0))
        //    {
        //        return segment.ToList();
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        //public UserEntity GetUser(string id)
        //{
        //    //CloudTableClient client = new CloudTableClient(storageAccount.TableEndpoint, storageAccount.Credentials);

        //    CloudTable table = client.GetTableReference(userTableName);
        //    var query = new TableQuery<UserEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, id));

        //    TableQuerySegment<UserEntity> segment = table.ExecuteQuerySegmented<UserEntity>(query, new TableContinuationToken());


        //    if (segment == null || segment.Results.Count == 0)
        //    {
        //        return null;
        //    }
        //    else
        //    {
        //        return segment.First();
        //    }
        //}

        //public UserEntity GetUser(int code)
        //{
        //    //CloudTableClient client = new CloudTableClient(storageAccount.TableEndpoint, storageAccount.Credentials);

        //    CloudTable table = client.GetTableReference(userTableName);
        //    var query = new TableQuery<UserEntity>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, code.ToString()));

        //    TableQuerySegment<UserEntity> segment = table.ExecuteQuerySegmented<UserEntity>(query, new TableContinuationToken());


        //    if (segment == null || segment.Results.Count == 0)
        //    {
        //        return null;
        //    }
        //    else
        //    {
        //        return segment.First();
        //    }
        //}

        //public UserEntity GetUserByEmail(string email)
        //{
        //    //CloudTableClient client = new CloudTableClient(storageAccount.TableEndpoint, storageAccount.Credentials);

        //    CloudTable table = client.GetTableReference(userTableName);
        //    var query = new TableQuery<UserEntity>().Where(TableQuery.GenerateFilterCondition("Email", QueryComparisons.Equal, email));

        //    TableQuerySegment<UserEntity> segment = table.ExecuteQuerySegmented<UserEntity>(query, new TableContinuationToken());
        //    if (segment == null || segment.Results.Count == 0)
        //    {
        //        return null;
        //    }
        //    else
        //    {
        //        return segment.First();
        //    }
        //}

        //public bool UserExists(string email)
        //{
        //    //CloudTableClient client = new CloudTableClient(storageAccount.TableEndpoint, storageAccount.Credentials);

        //    CloudTable table = client.GetTableReference(userTableName);
        //    var query = new TableQuery<UserEntity>().Where(TableQuery.GenerateFilterCondition("Email", QueryComparisons.Equal, email));

        //    TableQuerySegment<UserEntity> segment = table.ExecuteQuerySegmented<UserEntity>(query, new TableContinuationToken());
        //    return !(segment == null || segment.Results.Count == 0);
        //}

        //public bool CodeExists(int code)
        //{
        //    //CloudTableClient client = new CloudTableClient(storageAccount.TableEndpoint, storageAccount.Credentials);

        //    CloudTable table = client.GetTableReference(userTableName);
        //    var query = new TableQuery<UserEntity>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, code.ToString()));

        //    TableQuerySegment<UserEntity> segment = table.ExecuteQuerySegmented<UserEntity>(query, new TableContinuationToken());

        //    return !(segment == null || segment.Results.Count == 0);
        //}

        //public async Task UpdateAsync(UserEntity entity)
        //{
        //    //CloudTableClient client = new CloudTableClient(storageAccount.TableEndpoint, storageAccount.Credentials);

        //    Retry.ExecuteAsync(async () =>
        //    {
        //        CloudTable table = client.GetTableReference(userTableName);
        //        TableOperation updateOperation = TableOperation.InsertOrReplace(entity);
        //        await table.ExecuteAsync(updateOperation);

        //    }, TimeSpan.FromSeconds(2), 3);

        //}

        //public async Task DeleteAsync(PhoneEntity entity)
        //{
        //    //CloudTableClient client = new CloudTableClient(storageAccount.TableEndpoint, storageAccount.Credentials);

        //    Retry.ExecuteAsync(async () =>
        //    {
        //        CloudTable table = client.GetTableReference(userTableName);
        //        TableOperation deleteOperation = TableOperation.Delete(entity);
        //        await table.ExecuteAsync(deleteOperation);
        //    }, TimeSpan.FromSeconds(2), 3);
        //}

        public async Task UpdateAsync(PhoneEntity entity)
        {
            //CloudTableClient client = new CloudTableClient(storageAccount.TableEndpoint, storageAccount.Credentials);

            Retry.ExecuteAsync(async () =>
            {
                CloudTable table = client.GetTableReference(userTableName);
                TableOperation updateOperation = TableOperation.InsertOrReplace(entity);
                await table.ExecuteAsync(updateOperation);
            }, TimeSpan.FromSeconds(2), 3);

        }

        public void Update(PhoneEntity entity)
        {
            //CloudTableClient client = new CloudTableClient(storageAccount.TableEndpoint, storageAccount.Credentials);

            CloudTable table = client.GetTableReference(phoneTableName);
            TableOperation updateOperation = TableOperation.InsertOrReplace(entity);
            TableResult result = table.Execute(updateOperation);

            //Retry.Execute(() =>
            //{
            //    CloudTable table = client.GetTableReference(userTableName);
            //    TableOperation updateOperation = TableOperation.InsertOrReplace(entity);
            //    table.Execute(updateOperation);
            //}, TimeSpan.FromSeconds(2), 3);

        }


        public bool PhoneExists(string phone)
        {
            //CloudTableClient client = new CloudTableClient(storageAccount.TableEndpoint, storageAccount.Credentials);

            CloudTable table = client.GetTableReference(phoneTableName);
            var query = new TableQuery<PhoneEntity>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, phone));

            TableQuerySegment<PhoneEntity> segment = table.ExecuteQuerySegmented<PhoneEntity>(query, new TableContinuationToken());

            return !(segment == null || segment.Results.Count == 0);
        }

        public bool PhoneExists(PhoneEntity phone)
        {
            //CloudTableClient client = new CloudTableClient(storageAccount.TableEndpoint, storageAccount.Credentials);

            CloudTable table = client.GetTableReference(phoneTableName);
            var query = new TableQuery<PhoneEntity>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, phone.Phone));

            TableQuerySegment<PhoneEntity> segment = table.ExecuteQuerySegmented<PhoneEntity>(query, new TableContinuationToken());

            return !(segment == null || segment.Results.Count == 0);
        }

        public PhoneEntity GetPhone(string phone)
        {
            //CloudTableClient client = new CloudTableClient(storageAccount.TableEndpoint, storageAccount.Credentials);

            CloudTable table = client.GetTableReference(phoneTableName);
            var query = new TableQuery<PhoneEntity>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, phone));

            TableQuerySegment<PhoneEntity> segment = table.ExecuteQuerySegmented<PhoneEntity>(query, new TableContinuationToken());


            if (segment == null || segment.Results.Count == 0)
            {
                return null;
            }
            else
            {
                return segment.First();
            }
        }

        public PhoneEntity GetPhone(PhoneEntity phone)
        {
            //CloudTableClient client = new CloudTableClient(storageAccount.TableEndpoint, storageAccount.Credentials);

            CloudTable table = client.GetTableReference(phoneTableName);
            var query = new TableQuery<PhoneEntity>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, phone.Phone));

            TableQuerySegment<PhoneEntity> segment = table.ExecuteQuerySegmented<PhoneEntity>(query, new TableContinuationToken());


            if (segment == null || segment.Results.Count == 0)
            {
                return null;
            }
            else
            {
                return segment.First();
            }
        }

        public List<PhoneEntity> GetTruePhones()
        {
            //CloudTableClient client = new CloudTableClient(storageAccount.TableEndpoint, storageAccount.Credentials);

            CloudTable table = client.GetTableReference(phoneTableName);
            var query = new TableQuery<PhoneEntity>().Where(TableQuery.GenerateFilterConditionForBool("Activated", QueryComparisons.Equal, true));

            TableQuerySegment<PhoneEntity> segment = table.ExecuteQuerySegmented<PhoneEntity>(query, new TableContinuationToken());

            if (!(segment == null || segment.Results.Count == 0))
            {
                return segment.ToList();
            }
            else
            {
                return null;
            }
        }

        public List<PhoneEntity> GetFalsePhones()
        {
            //CloudTableClient client = new CloudTableClient(storageAccount.TableEndpoint, storageAccount.Credentials);

            CloudTable table = client.GetTableReference(phoneTableName);
            var query = new TableQuery<PhoneEntity>().Where(TableQuery.GenerateFilterConditionForBool("Activated", QueryComparisons.Equal, false));

            TableQuerySegment<PhoneEntity> segment = table.ExecuteQuerySegmented<PhoneEntity>(query, new TableContinuationToken());

            if (!(segment == null || segment.Results.Count == 0))
            {
                return segment.ToList();
            }
            else
            {
                return null;
            }
        }
    }
}
