namespace Thundercloud.Models;

public class SetGameRequest : GameActionRequest
{
	public SetGameRequest(string gameId) : base(gameId)
	{
	}
}