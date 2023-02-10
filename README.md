# thundercloud

A basic tabletop-inspired web app game to learn Azure technology.

## Plan

- [ ] Add UI to set which guild you are controlling.
- [ ] Add UI to display the currently selected action for each guild for the current turn.
- [ ] Add UI to end the current turn.
- [ ] Add UI to display the current turn number (treat each turn as a season, e.g. Year 3 - Fall, Year 7 - Winter, etc.)
- [x] Set up Azure Cosmos DB container to store the game state
- [ ] Set up Azure PubSub system
- [ ] Set up Azure Functions for the following routes/actions:
  - [x] HTTP triggers
    - [x] SetGame - sets the game you are playing by ID (creating a new game if it doesn't exist yet)
    - [x] EndTurn - applies current guild actions and completes objectives that are queued, generates new automated actions for the guilds for the next turn, generates new objectives if any were completed
    - [x] SetGuildTurnAction - set the turn action for the specified guild
    - [x] QueueObjectiveToComplete - sets the objective to be completed
  - [ ] Azure Cosmos DB Trigger
    - [ ] PropagateGameState - this will interact with the PubSub system and send updates to the game state out to all subscribers
      - This should be triggered by all of the HTTP triggers
- [ ] Hook up frontend to HTTP triggered functions using axios
- [ ] Hook up frontend to PubSub using ???
