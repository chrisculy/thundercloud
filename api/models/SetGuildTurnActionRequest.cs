namespace Thundercloud.Models;

public class SetGuildTurnActionRequest : GameActionRequest
{
	public SetGuildTurnActionRequest(string gameId, string guildName, string action) : base(gameId)
	{
		GuildName = guildName;
		Action =  action;
	}

	public string GuildName { get; }
	public string Action { get; }
}