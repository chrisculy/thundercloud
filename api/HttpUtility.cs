using System.Net;
using Microsoft.Azure.Functions.Worker.Http;

namespace ThundercloudApi;

public static class HttpUtility
{
    public static HttpResponseData CreateResponse(this HttpRequestData request, HttpStatusCode statusCode, string body)
    {
        var response = request.CreateResponse(statusCode);
        response.WriteString(body);
        return response;
    }
}
