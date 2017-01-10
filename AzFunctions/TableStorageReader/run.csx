#r "Microsoft.WindowsAzure.Storage"
using System.Net;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    var queryParamms = req.GetQueryNameValuePairs()
        .ToDictionary(p => p.Key, p => p.Value, StringComparer.OrdinalIgnoreCase);

    log.Info($"C# HTTP trigger function processed a request. {req.RequestUri}");

    HttpResponseMessage res = null;
    string customerReference;
    bool ok = false;
    if (queryParamms.TryGetValue("customerReference", out customerReference))
    {
        string settingName;
        if (queryParamms.TryGetValue("settingName", out settingName))
        {

            /* sayouevaldev
        MGpkGpZ/Vqs9u9aXADlJteA5E/FqvMBIzhywqyRrl2n8DHm+m8RuKhkVbSH7PyXYZSnLpFxQv8uWIJLS5vaofA==
        /pub/books.xml  */
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
            try
            {
                var virtualResults = table.ExecuteQuery(query);
                var settingValue = virtualResults.FirstOrDefault()["value"].StringValue;
                return req.CreateResponse(HttpStatusCode.OK, settingValue);
            }
            catch
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, "AppSetting couldn't be read");
            }
        }
    }
    return req.CreateResponse(HttpStatusCode.NotAcceptable, "Invalid Parameters");
}