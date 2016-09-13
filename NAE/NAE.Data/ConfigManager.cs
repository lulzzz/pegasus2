using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAE.Data
{
    public class ConfigManager
    {
        public ConfigManager()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            client = storageAccount.CreateCloudBlobClient();
        }

        private const string connectionString = "DefaultEndpointsProtocol=https;AccountName=pegasus2;AccountKey=M7Fh6ZzuL626vQ5YC2wcLMCBnEC3NmgXyoBuZjfwfrqCzFykJknKxfHmHxFE15br1/XQIJMkIVtGxwIwcDvoPw==;";
        private CloudBlobClient client;
        private const string blobUriString = "http://pegasus2.blob.core.windows.net/config/config.json";

        public List<Config> Read()
        {
            List<Config> list = null;
            
            Uri uri = new Uri(blobUriString);
            ICloudBlob blob = client.GetBlobReferenceFromServer(new StorageUri(uri));
            using (MemoryStream stream = new MemoryStream())
            {
                blob.DownloadToStream(stream);
                if(stream != null)
                {
                    stream.Position = 0;
                    byte[] data = stream.ToArray();
                    list = JsonConvert.DeserializeObject<List<Config>>(Encoding.UTF8.GetString(data));
                }
            }

            return list;
        }

        public void Write(List<Config> configList)
        {
            string jsonString = JsonConvert.SerializeObject(configList, Formatting.Indented);
            Uri uri = new Uri(blobUriString);
            ICloudBlob blob = client.GetBlobReferenceFromServer(new StorageUri(uri));
            byte[] byteArray = Encoding.UTF8.GetBytes(jsonString);
            blob.Properties.ContentType = "application/json";
            blob.UploadFromByteArray(byteArray, 0, byteArray.Length);
        }


    }
}
