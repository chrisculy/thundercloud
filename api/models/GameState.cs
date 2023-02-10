using System.Text.Json.Serialization;

namespace Thundercloud.Models;

public class ResourceSet
{
	public int Food { get; set; }
	public int Knowledge { get; set; }
	public int Power { get; set; }
	public int Stone { get; set; }
	public int Stormknowledge { get; set; }
	public int Stormpower { get; set; }
	public int Superfood { get; set; }
	public int Wood { get; set; }
}

public class Guilds
{
	[JsonPropertyName("Agriculture")]
	public Guild Agriculture { get; set; }
	[JsonPropertyName("Energy")]
	public Guild Energy { get; set; }
	[JsonPropertyName("Industry")]
	public Guild Industry { get; set; }
	[JsonPropertyName("Research")]
	public Guild Research { get; set; }
}

public class ResourceAmount
{
	public string Resource { get; set; }
	public int Amount { get; set; }
}

public class Guild
{
	public ResourceSet BuildingCostCommon { get; set; }
	public ResourceSet BuildingCostRare { get; set; }
	public int BuildingCountCommon { get; set; }
	public int BuildingCountRare { get; set; }
	public int ProductionRateCommon { get; set; }
	public int ProductionRateRare { get; set; }
	public ResourceAmount UpgradeCostCommon { get; set; }
	public ResourceAmount UpgradeCostRare { get; set; }
	public string Action { get; set; }
}

public static class ObjectiveRequirementType
{
	public const string Resource = "resource";
	public const string BuildingCommon = "buildingCommon";
	public const string BuildingRare = "buildingRare";
	public const string ProductionRateCommon = "productionRateCommon";
	public const string ProductionRateRare = "productionRateRare";
}

public class ObjectiveRequirement
{
  public string Type { get; set; }
  public string Value { get; set; }
  public int Amount { get; set; }
}

public class Objective
{
	public List<ObjectiveRequirement> Requirements { get; set; }
	public int Points { get; set; }
	public bool QueueForCompletion { get; set; }
}

public class GameState
{
	public string Id { get; set; }
	public Guilds Guilds { get; set; }
	public ResourceSet Resources { get; set; }
	public IReadOnlyList<Objective> Objectives { get; set; }
	public int Points { get; set; }
}