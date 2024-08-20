using SpaceExploration.Game.Events;
using SpaceExploration.Game.Planets;

public class GameStateService
{

    public List<Drone> Drones { get; } = new List<Drone>();

    public GameStateService()
    {
        Drones.Add(new Drone(Guid.NewGuid(), new Coordinate(0.5, 0.5), new Angle(45)));   
    }

    public void HandleDroneTurned(DroneTurned droneTurnedEvent)
    {
        var drone = Drones.Find(d => d.DroneId == droneTurnedEvent.DroneId);
        if (drone != null)
        {
            drone.Heading = droneTurnedEvent.Heading;
        }
    }

    public void AddDrone(DroneDropped drone)
    {
        Drones.Add(drone.Drone);
    }
}