namespace SpaceExploration.Game.Contracts.Drones.Messages;

public record ScanEnvironmentResult(Guid DroneId, List<DroneReading> SensorReadings);

public record DroneReading(Guid ReadingId, double Distance, double Heading);