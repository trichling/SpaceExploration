namespace SpaceExploration.Game.Contracts.Drones.Messages;

public record ShotResult(Guid DroneId, ShotOutcome Outcome, Guid? TargetId, Guid? TargetSignature)
{
    public ShotResult() : this(Guid.Empty, ShotOutcome.ShootingDroneNotFound, null, null)
    {
    }

    public ShotResult(Guid droneId, ShotOutcome outcome) : this(droneId, outcome, null, null)
    {
    }
    public ShotResult(Guid droneId, ShotOutcome outcome, Guid targetId) : this(droneId, outcome, targetId, null)
    {
    }
}

public enum ShotOutcome
{
    ShootingDroneNotFound,
    NoSensorReadings,
    NoSensorReadingForTargetId,
    TargetDroneNotFound,
    Hit,
    Destroyed
}