#r "Microsoft.WindowsAzure.Storage"
#r "Newtonsoft.Json"

using System.Net;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

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
        var accountName = "YOUR_ACCOUNT_NAME";
        var accountKey = "YOUR_ACCOUNT_KEY";
        var storageAccount = CloudStorageAccount.Parse($"DefaultEndpointsProtocol=https;AccountName={accountName};AccountKey={accountKey};");
        var tableClient = storageAccount.CreateCloudTableClient();
        var table = tableClient.GetTableReference("AppSettings");

        var customerFilter = TableQuery.GenerateFilterCondition(
            "PartitionKey",
            QueryComparisons.Equal,
            customerReference);

        var combinedFilter = customerFilter;

        string settingName;
        if (queryParamms.TryGetValue("settingName", out settingName))
        {
            var appSettingFilter = TableQuery.GenerateFilterCondition(
                "RowKey",
                QueryComparisons.Equal,
                settingName);

            combinedFilter = TableQuery.CombineFilters(
                customerFilter,
                TableOperators.And,
                appSettingFilter
                );
        }

        var query = new TableQuery().Where(combinedFilter);
        try
        {
            var virtualResults = table.ExecuteQuery(query);
            var settings =
            (from setting in virtualResults
            select new
            {
                name = setting.RowKey,
                value = setting["value"].StringValue
            });
            var jsonSettings = JsonConvert.SerializeObject(settings);
            return req.CreateResponse(HttpStatusCode.OK, jsonSettings);
        }
        catch
        {
            return req.CreateResponse(HttpStatusCode.InternalServerError, "AppSetting couldn't be read.");
        }
    }
    return req.CreateResponse(HttpStatusCode.NotAcceptable, "Invalid Parameters");
}