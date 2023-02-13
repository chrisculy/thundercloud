using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace ThundercloudApi;

public class SignalRFunctions
{
	 private static readonly HttpClient HttpClient = new();
    private static string Etag = string.Empty;
    private static int StarCount = 0;

    [Function("negotiate")]
    public static HttpResponseData Negotiate([HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequestData req,
        [SignalRConnectionInfoInput(HubName = c_hubName)] string connectionInfo)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json");
        response.WriteString(connectionInfo);
        return response;
    }

    [Function("broadcast")]
    [SignalROutput(HubName = c_hubName)]
    public static async Task<SignalRMessageAction> Broadcast([TimerTrigger("*/5 * * * * *")] TimerInfo timerInfo)
    {
        return new SignalRMessageAction("newMessage", new object[] { $"Thundercloud SignalR message #{s_messageNumber++}" });
    }

    const string c_hubName = "thundercloud";

	static ulong s_messageNumber = 1;
}