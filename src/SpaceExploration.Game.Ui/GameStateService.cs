namespace SpaceExploration.Game.Ui;

using SpaceExploration.Game.Events;
using SpaceExploration.Game.Planets;

public class GameStateService
{

    public event EventHandler StateChanged;

    public List<Drone> Drones { get; } 
    public List<DroneShot> Shots { get; } 


    public GameStateService()
    {
        Drones = new List<Drone>();
        Shots = new List<DroneShot>();
    }

    private GameStateService(List<Drone> drones, List<DroneShot> shots)
    {
        Drones = drones;
        Shots = shots;
    }   

    public void HandleDroneDropped(DroneDropped message)
    {

        Console.WriteLine(Drones.Count);
        if (!Drones.Any(d => d.DroneId == message.DroneId))
        {
            Drones.Add(new Drone(message.DroneId, new Coordinate(message.X, message.Y), new Angle(message.Heading)));
        }

        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void HandleDroneTurned(DroneTurned message)
    {

        if (!Drones.Any(d => d.DroneId == message.DroneId))
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

        if (!Drones.Any(d => d.DroneId == message.DroneId))
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

    internal void HandleDroneHit(SpaceExploration.Game.Contracts.Events.DroneHit message)
    {

        if (!Drones.Any(d => d.DroneId == message.ShootingDroneId))
        {
            var newDrone = new Drone(message.ShootingDroneId);
            Drones.Add(newDrone);
        }

        if (!Drones.Any(d => d.DroneId == message.TargetDroneId))
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
        Shots.Add(new DroneShot { Shooter = shooter, Target = target });

        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    internal void HandleDroneDestroyed(SpaceExploration.Game.Contracts.Events.DroneDestroyed message)
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
        //Shots.RemoveAll(s => s.Drawn);
        Shots.Clear();
    }

    internal GameStateService Freeze()
    {
        return new GameStateService
        (
            Drones.ToList(),
            Shots.ToList()
        );
    }
}
