namespace SpaceExploration.Game.Ui;

using SpaceExploration.Game.Events;
using System.Collections.Concurrent; // Add this using directive

public class GameStateService
{
    readonly ConcurrentDictionary<Guid, GameState> gameStates = new(); // Change Dictionary to ConcurrentDictionary

    public GameState GetGameState(Guid planetId)
    {
        var gameState = gameStates.GetOrAdd(planetId, new GameState(planetId));
        return gameState.Freeze();
    }

    public void HandleDroneDropped(DroneDropped message)
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

    internal void HandleDroneHit(Contracts.Events.DroneHit message)
    {
        var gameState = gameStates.GetOrAdd(message.PlanetId, new GameState(message.PlanetId));
        gameState.HandleDroneHit(message);
    }

    internal void HandleDroneDestroyed(Contracts.Events.DroneDestroyed message)
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
