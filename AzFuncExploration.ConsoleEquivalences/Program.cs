using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace XmlContentFromBlob
{
    class Program
    {
        static void Main(string[] args)
        {
            // XMLContent Reading
            var xmlContent = XmlContent("books.xml").Result;
            Console.WriteLine(xmlContent);


            //Table setting reading
            /*var customerName = "Cliente1";
            var appSettingName = "StringConnectionMainDB";
            var appSettingValue = TableAppSetting(customerName, appSettingName);
            Console.WriteLine($"The {appSettingName} for {customerName} is: {appSettingValue}");*/




            Console.ReadKey();
        }

        public async static Task<string> XmlContent(string blobName)
        {
            /* sayouevaldev
            MGpkGpZ/Vqs9u9aXADlJteA5E/FqvMBIzhywqyRrl2n8DHm+m8RuKhkVbSH7PyXYZSnLpFxQv8uWIJLS5vaofA==
            /pub/books.xml  */
            var accountName = "sayouevaldev";
            var accountKey = "MGpkGpZ/Vqs9u9aXADlJteA5E/FqvMBIzhywqyRrl2n8DHm+m8RuKhkVbSH7PyXYZSnLpFxQv8uWIJLS5vaofA==";
            var containerReference = "pub";
            var storageAccount = CloudStorageAccount.Parse($"DefaultEndpointsProtocol=https;AccountName={accountName};AccountKey={accountKey};");
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerReference);
            var blob = container.GetBlobReference(blobName);
            if (blob.Exists())
            {
                var blobByteLength = blob.Properties.Length;
                byte[] blobBytes = new byte[blobByteLength];
                await blob.DownloadToByteArrayAsync(blobBytes, 0);
                var xmlContent = System.Text.Encoding.Default.GetString(blobBytes);
                return xmlContent;
            }
            else return "ERROR";
        }

        public static string TableAppSetting(string customerReference, string settingName)
        {
            var accountName = "sayouevaldev";
            var accountKey = "MGpkGpZ/Vqs9u9aXADlJteA5E/FqvMBIzhywqyRrl2n8DHm+m8RuKhkVbSH7PyXYZSnLpFxQv8uWIJLS5vaofA==";
            var storageAccount = CloudStorageAccount.Parse($"DefaultEndpointsProtocol=https;AccountName={accountName};AccountKey={accountKey};");
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("AppSettings");

            var customerFilter = TableQuery.GenerateFilterCondition(
                "PartitionKey",
                QueryComparisons.Equal,
                customerReference);

            var appSettingFilter = TableQuery.GenerateFilterCondition(
                "RowKey",
                QueryComparisons.Equal,
                settingName);

            var combinedFilter = TableQuery.CombineFilters(
                customerFilter,
                TableOperators.And,
                appSettingFilter
                );

            var query = new TableQuery().Where(combinedFilter);
            var virtualResults = table.ExecuteQuery(query);
            var settingValue = virtualResults.FirstOrDefault()["value"].StringValue;
            return settingValue;
        }
    }
}