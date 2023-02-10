using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using Microsoft.Azure.WebPubSub.Common;
using Microsoft.Extensions.Logging;

namespace ThundercloudPubSub;

public class PubSubFunctions
{
	public PubSubFunctions(ILoggerFactory loggerFactory)
	{
		_logger = loggerFactory.CreateLogger<PubSubFunctions>();
	}

	[FunctionName("negotiate")]
	public WebPubSubConnection Negotiate(
		[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest request,
		[WebPubSubConnection(Hub = c_hubName, UserId = "{headers.x-ms-client-principal-name}")] WebPubSubConnection connection)
	{
		_logger.LogInformation("Connecting...");
		return connection;
	}

	[FunctionName("message")]
	public static async Task<UserEventResponse> Message(
		[WebPubSubTrigger(c_hubName, WebPubSubEventType.User, "message")] UserEventRequest request,
		BinaryData data,
		WebPubSubDataType dataType,
		[WebPubSub(Hub = c_hubName)] IAsyncCollector<WebPubSubAction> actions)
	{
		await actions.AddAsync(WebPubSubAction.CreateSendToAllAction(
			BinaryData.FromString($"[{request.ConnectionContext.UserId}] {data.ToString()}"),
			dataType));
		return new UserEventResponse
		{
			Data = BinaryData.FromString("[SYSTEM] ack"),
			DataType = WebPubSubDataType.Text
		};
	}

	private const string c_hubName = "thundercloud";
	
	private readonly ILogger _logger;
}
