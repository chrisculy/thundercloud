namespace Thundercloud.Models;

public class QueueObjectiveToCompleteRequest : GameActionRequest
{
	public QueueObjectiveToCompleteRequest(string gameId, int objectiveIndex) : base(gameId)
	{
		ObjectiveIndex = objectiveIndex;
	}

	public int ObjectiveIndex { get; }
}