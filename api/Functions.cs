using System.Net;
using Azure.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Thundercloud.Models;

namespace ThundercloudApi;

public class Functions
{
    public Functions(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<Functions>();
    }

    [Function("SetGame")]
    public async Task<HttpResponseData> SetGameAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData httpRequest)
    {
        var request = await httpRequest.ReadFromJsonAsync<SetGameRequest>();
        if (request == null)
        {
            return httpRequest.CreateResponse(HttpStatusCode.BadRequest, "Failed to parse SetGameRequest properly.");
        }

        using var client = CreateCosmosClient();
        var gameState = await CreateGameIfNotExistsAsync(client, request.GameId);

        var response = httpRequest.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(gameState);
        return response;
    }

    [Function("EndTurn")]
    public async Task<HttpResponseData> EndTurnAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData httpRequest)
    {
        var request = await httpRequest.ReadFromJsonAsync<GameActionRequest>();
        if (request == null)
        {
            return httpRequest.CreateResponse(HttpStatusCode.BadRequest, "Failed to parse GameActionRequest properly.");
        }

        // check if game does not exist in the DB
        using var client = CreateCosmosClient();
        var gameState = await GetGameStateAsync(client, request.GameId);
        if (gameState == null)
        {
            return httpRequest.CreateResponse(HttpStatusCode.BadRequest, $"Failed to retrieve game state for ID '{request.GameId}'.");
        }

        // apply guild actions
        ApplyGuildAction(gameState, "Agriculture");
        ApplyGuildAction(gameState, "Energy");
        ApplyGuildAction(gameState, "Industry");
        ApplyGuildAction(gameState, "Research");

        UpdateObjectives(gameState);

        await UpdateGameStateAsync(client, gameState);
        return httpRequest.CreateResponse(HttpStatusCode.OK);
    }

    [Function("SetGuildTurnAction")]
    public async Task<HttpResponseData> SetGuildTurnActionAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData httpRequest)
    {
        var request = await httpRequest.ReadFromJsonAsync<SetGuildTurnActionRequest>();
        if (request == null)
        {
            return httpRequest.CreateResponse(HttpStatusCode.BadRequest, "Failed to parse SetGuildTurnActionRequest properly.");
        }

        // check if game does not exist in the DB
        using var client = CreateCosmosClient();
        var gameState = await GetGameStateAsync(client, request.GameId);
        if (gameState == null)
        {
            return httpRequest.CreateResponse(HttpStatusCode.BadRequest, $"Failed to retrieve game state for ID '{request.GameId}'.");
        }
        
        // set the specified action for the specified guild
        switch (request.GuildName)
        {
            case "Agriculture":
                gameState.Guilds.Agriculture.Action = request.Action;
                break;
            case "Energy":
                gameState.Guilds.Energy.Action = request.Action;
                break;
            case "Industry":
                gameState.Guilds.Industry.Action = request.Action;
                break;
            case "Research":
                gameState.Guilds.Research.Action = request.Action;
                break;
            default:
                return httpRequest.CreateResponse(HttpStatusCode.BadRequest, $"Invalid guild '{request.GuildName}'.");
        }

        await UpdateGameStateAsync(client, gameState);
        return httpRequest.CreateResponse(HttpStatusCode.OK);
    }

    [Function("QueueObjectiveToComplete")]
    public async Task<HttpResponseData> QueueObjectiveToCompleteAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData httpRequest)
    {
        var request = await httpRequest.ReadFromJsonAsync<QueueObjectiveToCompleteRequest>();
        if (request == null)
        {
            return httpRequest.CreateResponse(HttpStatusCode.BadRequest, "Failed to parse QueueObjectiveToCompleteRequest properly.");
        }

        // check if game does not exist in the DB
        using var client = CreateCosmosClient();
        var gameState = await GetGameStateAsync(client, request.GameId);
        if (gameState == null)
        {
            return httpRequest.CreateResponse(HttpStatusCode.BadRequest, $"Failed to retrieve game state for ID '{request.GameId}'.");
        }
        
        if (request.ObjectiveIndex >= gameState.Objectives.Count)
        {
            return httpRequest.CreateResponse(HttpStatusCode.BadRequest, $"Invalid objective index '{request.ObjectiveIndex}'.");
        }

        // queue the specified objective to be completed when the turn ends
        gameState.Objectives[request.ObjectiveIndex].QueueForCompletion = true;
        await UpdateGameStateAsync(client, gameState);

        return httpRequest.CreateResponse(HttpStatusCode.OK);
    }

    private static async Task<GameState> CreateGameIfNotExistsAsync(CosmosClient client, string id)
    {
        var container = GetContainer(client);
        var gameState = await GetGameStateAsync(client, id);
        if (gameState == null)
        {
            gameState = await GetDefaultGameStateAsync(client);
            gameState.Id = id;
            gameState.Objectives = new List<Objective>
            {
                GenerateObjective(gameState),
                GenerateObjective(gameState),
                GenerateObjective(gameState),
            };
            gameState = await container.CreateItemAsync(gameState);
        }

        return gameState;
    }

    private static async Task<GameState?> GetGameStateAsync(CosmosClient client, string gameId)
    {
        var query = new QueryDefinition("select * from thundercloud t where t.id = @gameId").WithParameter("@gameId", gameId);
        using var feedIterator = GetContainer(client).GetItemQueryIterator<GameState>(query, requestOptions: new QueryRequestOptions{ MaxItemCount = 1 });
        
        if (feedIterator.HasMoreResults)
        {
            var results = await feedIterator.ReadNextAsync();
            if (results.Count == 0)
            {
                return null;
            }

            return results.First(x => x.Id == gameId);
        }
        return null;
    }

    private static async Task UpdateGameStateAsync(CosmosClient client, GameState gameState)
    {
        await GetContainer(client).ReplaceItemAsync(gameState, gameState.Id);
    }

    private static async Task<GameState> GetDefaultGameStateAsync(CosmosClient client)
    {
        return (await GetGameStateAsync(client, c_defaultGameState)) ?? throw new InvalidOperationException("Failed to retrieve default game state from DB.");
    }

    private static void ApplyGuildAction(GameState gameState, string guildName)
    {
        var guild = GetGuild(gameState, guildName);

        switch (guild.Action)
        {
            case "produceCommon":
            {
                var amount = guild.BuildingCountCommon * guild.ProductionRateCommon;
                switch (guildName)
                {
                    case "Agriculture":
                        gameState.Resources.Food += amount;
                        break;
                    case "Energy":
                        gameState.Resources.Power += amount;
                        break;
                    case "Industry":
                        gameState.Resources.Wood += amount;
                        break;
                    case "Research":
                        gameState.Resources.Knowledge += amount;
                        break;
                }
                break;
            }
            case "produceRare":
            {
                var amount = guild.BuildingCountRare * guild.ProductionRateRare;
                switch (guildName)
                {
                    case "Agriculture":
                        gameState.Resources.Superfood += amount;
                        break;
                    case "Energy":
                        gameState.Resources.Stormpower += amount;
                        break;
                    case "Industry":
                        gameState.Resources.Stone += amount;
                        break;
                    case "Research":
                        gameState.Resources.Stormknowledge += amount;
                        break;
                }
                break;
            }
            case "buildCommon":
                guild.BuildingCountCommon++;
                break;
            case "buildRare":
                guild.BuildingCountRare++;
                break;
            case "upgradeCommon":
                guild.ProductionRateCommon++;
                switch (guild.UpgradeCostCommon.Resource)
                {
                    case "food":
                        gameState.Resources.Food -= guild.UpgradeCostCommon.Amount;
                        break;
                    case "knowledge":
                        gameState.Resources.Knowledge -= guild.UpgradeCostCommon.Amount;
                        break;
                    case "power":
                        gameState.Resources.Power -= guild.UpgradeCostCommon.Amount;
                        break;
                    case "wood":
                        gameState.Resources.Wood -= guild.UpgradeCostCommon.Amount;
                        break;
                }
                break;
            case "upgradeRare":
                guild.ProductionRateRare++;
                switch (guild.UpgradeCostRare.Resource)
                {
                    case "stone":
                        gameState.Resources.Stone -= guild.UpgradeCostRare.Amount;
                        break;
                    case "stormknowledge":
                        gameState.Resources.Stormknowledge -= guild.UpgradeCostRare.Amount;
                        break;
                    case "stormpower":
                        gameState.Resources.Stormpower -= guild.UpgradeCostRare.Amount;
                        break;
                    case "superfood":
                        gameState.Resources.Superfood -= guild.UpgradeCostRare.Amount;
                        break;
                }
                break;
        }

        // generate a random next action
        guild.Action = s_guildActions[s_random.Next(s_guildActions.Count)];
    }

    private static void UpdateObjectives(GameState gameState)
    {
        var objectivesCompleted = false;
        foreach (var objective in gameState.Objectives)
        {
            if (objective.QueueForCompletion)
            {
                var objectiveSatisfied = true;
                foreach (var requirement in objective.Requirements)
                {
                    if (requirement.Type == ObjectiveRequirementType.Resource)
                    {
                        switch (requirement.Value)
                        {
                            case "food":
                                if (gameState.Resources.Food >= requirement.Amount)
                                    gameState.Resources.Food -= requirement.Amount;
                                else
                                    objectiveSatisfied = false;
                                break;
                            case "knowledge":
                                if (gameState.Resources.Knowledge >= requirement.Amount)
                                    gameState.Resources.Knowledge -= requirement.Amount;
                                else
                                    objectiveSatisfied = false;
                                break;
                            case "power":
                                if (gameState.Resources.Power >= requirement.Amount)
                                    gameState.Resources.Power -= requirement.Amount;
                                else
                                    objectiveSatisfied = false;
                                break;
                            case "stone":
                                if (gameState.Resources.Stone >= requirement.Amount)
                                    gameState.Resources.Stone -= requirement.Amount;
                                else
                                    objectiveSatisfied = false;
                                break;
                            case "stormknowledge":
                                if (gameState.Resources.Stormknowledge >= requirement.Amount)
                                    gameState.Resources.Stormknowledge -= requirement.Amount;
                                else
                                    objectiveSatisfied = false;
                                break;
                            case "stormpower":
                                if (gameState.Resources.Stormpower >= requirement.Amount)
                                    gameState.Resources.Stormpower -= requirement.Amount;
                                else
                                    objectiveSatisfied = false;
                                break;
                            case "superfood":
                                if (gameState.Resources.Superfood >= requirement.Amount)
                                    gameState.Resources.Superfood -= requirement.Amount;
                                else
                                    objectiveSatisfied = false;
                                break;
                            case "wood":
                                if (gameState.Resources.Wood >= requirement.Amount)
                                    gameState.Resources.Wood -= requirement.Amount;
                                else
                                    objectiveSatisfied = false;
                                break;
                        }
                    }
                    else
                    {
                        var guild = GetGuild(gameState, requirement.Value);
                        if (requirement.Type == ObjectiveRequirementType.BuildingCommon && guild.BuildingCountCommon < requirement.Amount)
                        {
                            objectiveSatisfied = false;
                        }
                        else if (requirement.Type == ObjectiveRequirementType.BuildingRare && guild.BuildingCountRare < requirement.Amount)
                        {
                            objectiveSatisfied = false;
                        }
                        else if (requirement.Type == ObjectiveRequirementType.ProductionRateCommon && guild.ProductionRateCommon < requirement.Amount)
                        {
                            objectiveSatisfied = false;
                        }
                        else if (requirement.Type == ObjectiveRequirementType.ProductionRateRare && guild.ProductionRateRare < requirement.Amount)
                        {
                            objectiveSatisfied = false;
                        }
                    }
                }

                if (objectiveSatisfied)
                {
                    objectivesCompleted = true;
                    gameState.Points += objective.Points;
                }
            }
        }

        if (objectivesCompleted)
        {
            gameState.Objectives = new List<Objective>()
            {
                GenerateObjective(gameState),
                GenerateObjective(gameState),
                GenerateObjective(gameState),
            };
        }
    }

    private static Guild GetGuild(GameState gameState, string guildName)
    {
        return guildName == "Agriculture" ? gameState.Guilds.Agriculture :
            guildName == "Energy" ? gameState.Guilds.Energy :
            guildName == "Industry" ? gameState.Guilds.Industry : gameState.Guilds.Research;
    }

    private static Objective GenerateObjective(GameState gameState)
    {
        var requirementCount = s_random.Next(2) + 1;
        var requirements = new List<ObjectiveRequirement>();
        var points = 0;
        for (var i = 0; i < requirementCount; i++)
        {
            // 0-1 : building, 2-3 : production rate, 4-6: resource
            var requirementTypeRandomizer = s_random.Next(5);
            var guildName = s_guildNames[s_random.Next(s_guildNames.Count)];
            var guild = GetGuild(gameState, guildName);
            switch (requirementTypeRandomizer)
            {
                case 0:
                {
                    var amount = s_random.Next(guild.BuildingCountCommon) + 1;
                    points += 5 * amount;
                    requirements.Add(new ObjectiveRequirement
                    {
                        Type = ObjectiveRequirementType.BuildingCommon,
                        Value = guildName,
                        Amount = guild.BuildingCountCommon + amount,
                    });
                    break;
                }
                case 1:
                {
                    var amount = s_random.Next(guild.BuildingCountRare) + 1;
                    points += 5 * amount;
                    requirements.Add(new ObjectiveRequirement
                    {
                        Type = ObjectiveRequirementType.BuildingRare,
                        Value = guildName,
                        Amount = guild.BuildingCountRare + amount,
                    });
                    break;
                }
                case 2:
                {
                    var amount = s_random.Next((int)Math.Round(guild.ProductionRateCommon * 1.5));
                    points += 3*amount;
                    requirements.Add(new ObjectiveRequirement
                    {
                        Type = ObjectiveRequirementType.ProductionRateCommon,
                        Amount = guild.ProductionRateCommon + amount,
                        Value = guildName
                    });
                    break;
                }
                case 3:
                {
                    var amount = s_random.Next((int)Math.Round(guild.ProductionRateRare * 1.5));
                    points += 3*amount;
                    requirements.Add(new ObjectiveRequirement
                    {
                        Type = ObjectiveRequirementType.ProductionRateRare,
                        Amount = guild.ProductionRateRare + amount,
                        Value = guildName
                    });
                    break;
                }
                case 4:
                case 5:
                case 6:
                default:
                {
                    var amount = 5 + s_random.Next(5) * 5;
                    points += 2 * amount;
                    requirements.Add(new ObjectiveRequirement
                    {
                        Type = ObjectiveRequirementType.Resource,
                        Amount = amount,
                        Value = s_resources[s_random.Next(s_resources.Count)]
                    });
                    break;
                }
            }
        }

        return new Objective()
        {
            Requirements = requirements,
            Points = points,
            QueueForCompletion = false,
        };
    }

    private static readonly List<string> s_guildActions = new List<string>
    {
        "produceCommon",
        "produceRare",
        "buildCommon",
        "buildRare",
        "upgradeCommon",
        "upgradeRare",
    };

    private static readonly List<String> s_guildNames = new List<string>
    {
        "Agriculture",
        "Energy",
        "Industry",
        "Research",
    };

    private static readonly List<String> s_resources = new List<string>
    {
        "food",
        "knowledge",
        "power",
        "stone",
        "stormknowledge",
        "stormpower",
        "superfood",
        "wood",
    };

    private static Random s_random = new Random();

    private static CosmosClient CreateCosmosClient() => new(
        accountEndpoint: Environment.GetEnvironmentVariable("THUNDERCLOUD_DB_URI"),
        tokenCredential: new DefaultAzureCredential(),
        clientOptions: new CosmosClientOptions { SerializerOptions = new CosmosSerializationOptions { IgnoreNullValues = true, PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase } });

    private static Container GetContainer(CosmosClient client) => client.GetContainer(c_databaseId, c_containerId) ?? throw new InvalidOperationException($"Failed to get required CosmosDB container '{c_containerId}'.");

    private static PartitionKey s_partitionKey = new PartitionKey("id");

    private const string c_defaultGameState = "__default__";
    private const string c_containerId = "thundercloud";
    private const string c_databaseId = "thundercloud";

    private readonly ILogger _logger;
}
