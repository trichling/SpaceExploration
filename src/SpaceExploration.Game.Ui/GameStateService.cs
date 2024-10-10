namespace SpaceExploration.Game.Ui;

using SpaceExploration.Game.Contracts.Drones.Events;
using SpaceExploration.Game.Contracts.Planets.Commands;
using SpaceExploration.Game.Contracts.Planets.Events;
using SpaceExploration.Game.Contracts.Planets.Messages;

using System.Collections.Concurrent; // Add this using directive

public class GameStateService
{
    readonly ConcurrentDictionary<Guid, GameState> gameStates = new(); // Change Dictionary to ConcurrentDictionary

    public GameState GetGameState(Guid planetId)
    {
        var gameState = gameStates.GetOrAdd(planetId, new GameState(planetId));
        return gameState.Freeze();
    }

    internal void HandleCatchUpResponse(CatchUpResponse message)
    {
        var gameState = gameStates.GetOrAdd(message.PlanetId, new GameState(message.PlanetId));
        gameState.HandleCatchUpResponse(message);
    }

    public void HandleDroneDropped(Contracts.Planets.Events.DroneDropped message)
    {
        var gameState = gameStates.GetOrAdd(message.PlanetId, new GameState(message.PlanetId));
        gameState.HandleDroneDropped(message);
    }

    public void HandleDroneTurned(DroneTurned message)
    {
        var gameState = gameStates.GetOrAdd(message.PlanetId, new GameState(message.PlanetId));
        gameState.HandleDroneTurned(message);
    }

    internal void HandleDroneMoved(DroneMoved message)
    {
        var gameState = gameStates.GetOrAdd(message.PlanetId, new GameState(message.PlanetId));
        gameState.HandleDroneMoved(message);
    }

    internal void HandleDroneHit(DroneHit message)
    {
        var gameState = gameStates.GetOrAdd(message.PlanetId, new GameState(message.PlanetId));
        gameState.HandleDroneHit(message);
    }

    internal void HandleDroneDestroyed(DroneDestroyed message)
    {
        var gameState = gameStates.GetOrAdd(message.PlanetId, new GameState(message.PlanetId));
        gameState.HandleDroneDestroyed(message);
    }

    internal void RemoveShots(Guid planetId)
    {
        var gameState = gameStates.GetOrAdd(planetId, new GameState(planetId));
        gameState.RemoveDrawnShots();
    }


}
