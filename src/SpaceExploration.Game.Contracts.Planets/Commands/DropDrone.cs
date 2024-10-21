namespace SpaceExploration.Game.Contracts.Planets.Commands;
public record DropDrone(Guid DroneId, Guid DroneSignature, Guid PlanetId, string DroneType = "", string DroneName = "");