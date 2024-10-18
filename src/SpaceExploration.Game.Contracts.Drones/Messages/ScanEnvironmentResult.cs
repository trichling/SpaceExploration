namespace SpaceExploration.Game.Contracts.Drones.Messages;

public record ScanEnvironmentResult(Guid DroneId, List<DroneReading> SensorReadings);

public record DroneReading(Guid ReadingId, Guid DroneSignature, double Distance, double Heading);