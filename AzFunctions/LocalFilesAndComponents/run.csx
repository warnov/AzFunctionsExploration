#r "AzFuncExploration.CustomTypes.dll"
using CustomTypes;
using System.Net;
using System.IO;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($"C# HTTP trigger function processed a request. RequestUri={req.RequestUri}");

    // parse query parameter
    string name = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
        .Value;

    // Get request body
    dynamic data = await req.Content.ReadAsAsync<object>();

    // Set name to query string or body data
    name = name ?? data?.name;

    //Using external library
    ComplexNumber complex = new ComplexNumber(3, 5);

    //Loading the contents of a local file
    var fileText = File.ReadAllText("./LocalFilesAndComponents/hello.txt");

    return name == null
        ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
        : req.CreateResponse(HttpStatusCode.OK,
            $"Hello {name} here is a complex number {complex}. And the text content is: {fileText}.");
}