namespace SpaceExploration.Game.Contracts.Commands;
public record DropDrone(Guid DroneId, Guid PlanetId, string DroneType = "", string DroneName = "");