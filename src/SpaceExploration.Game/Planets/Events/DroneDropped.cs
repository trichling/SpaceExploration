using SpaceExploration.Game.Planets;

namespace SpaceExploration.Game.Events;

public record DroneDropped(Guid PlanetId, Drone Drone);