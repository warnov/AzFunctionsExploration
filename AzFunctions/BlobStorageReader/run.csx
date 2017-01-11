#r "Microsoft.WindowsAzure.Storage"
using System.Net;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($"C# HTTP trigger function processed a request. RequestUri={req.RequestUri}");

    // parse query parameter
    string blobName = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "blobName", true) == 0)
        .Value;


    var accountName = "YOUR_ACCOUNT_NAME";
    var accountKey = "YOUR_ACCOUNT_KEY";
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
        return req.CreateResponse(HttpStatusCode.OK, xmlContent);
    }
    else return req.CreateResponse(HttpStatusCode.InternalServerError, "Blob not being able to be read");
}