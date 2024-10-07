namespace SpaceExploration.Game.Ui;

using SpaceExploration.Game.Contracts.Drones.Events;
using SpaceExploration.Game.Contracts.Planets.Events;
using SpaceExploration.Game.Planets;

public class GameState
{

    public event EventHandler StateChanged;

    public List<Drone> Drones { get; }
    public List<DroneShot> Shots { get; }
    public Guid WorldId { get; }

    public GameState(Guid worldId)
    {
        Drones = new List<Drone>();
        Shots = new List<DroneShot>();
        WorldId = worldId;
    }

    private GameState(Guid worldId, List<Drone> drones, List<DroneShot> shots)
    {
        WorldId = worldId;
        Drones = drones;
        Shots = shots;
    }

    public void HandleDroneDropped(Contracts.Planets.Events.DroneDropped message)
    {

        Console.WriteLine(Drones.Count);
        if (!Drones.Exists(d => d.DroneId == message.DroneId))
        {
            Drones.Add(new Drone(message.DroneId, new Coordinate(message.X, message.Y), new Angle(message.Heading)));
        }

        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void HandleDroneTurned(DroneTurned message)
    {

        if (!Drones.Exists(d => d.DroneId == message.DroneId))
        {
            var newDrone = new Drone(message.DroneId)
            {
                Heading = new Angle(message.Heading)
            };
            Drones.Add(newDrone);
        }

        var drone = Drones.Find(d => d.DroneId == message.DroneId);
        if (drone != null)
        {
            drone.Heading = new Angle(message.Heading);
        }

        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    internal void HandleDroneMoved(DroneMoved message)
    {

        if (!Drones.Exists(d => d.DroneId == message.DroneId))
        {
            var newDrone = new Drone(message.DroneId)
            {
                Position = new Coordinate(message.X, message.Y)
            };
            Drones.Add(newDrone);
        }

        var drone = Drones.Find(d => d.DroneId == message.DroneId);
        if (drone != null)
        {
            drone.Position = new Coordinate(message.X, message.Y);
        }

        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    internal void HandleDroneHit(DroneHit message)
    {

        if (!Drones.Exists(d => d.DroneId == message.ShootingDroneId))
        {
            var newDrone = new Drone(message.ShootingDroneId);
            Drones.Add(newDrone);
        }

        if (!Drones.Exists(d => d.DroneId == message.TargetDroneId))
        {
            var newDrone = new Drone(message.TargetDroneId);
            Drones.Add(newDrone);
        }

        var drone = Drones.Find(d => d.DroneId == message.TargetDroneId);
        if (drone != null)
        {
            drone.Health = message.RemainingHealth;
        }

        var shooter = Drones.Find(d => d.DroneId == message.ShootingDroneId);
        var target = Drones.Find(d => d.DroneId == message.TargetDroneId);
        Shots.Add(new DroneShot { Shooter = shooter, Target = target, Cycles = 0 });

        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    internal void HandleDroneDestroyed(DroneDestroyed message)
    {

        var drone = Drones.Find(d => d.DroneId == message.DroneId);
        if (drone != null)
        {
            Drones.Remove(drone);
        }

        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    internal void RemoveDrawnShots()
    {
        foreach (var shot in Shots)
        {
            shot.Cycles++;
        }

        Shots.RemoveAll(s => s.Cycles > 10);
    }

    internal GameState Freeze()
    {
        return new GameState
        (
            WorldId,
            Drones.ToList(),
            Shots.ToList()
        );
    }
}
