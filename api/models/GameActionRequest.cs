namespace Thundercloud.Models;

public class GameActionRequest
{
	public GameActionRequest(string gameId)
	{
		GameId = gameId;
	}

	public string GameId { get; }
}