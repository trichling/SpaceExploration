using SpaceExploration.Game.Events;
using SpaceExploration.Game.Planets;

public class GameStateService
{
    private readonly List<Drone> _drones = new List<Drone>();

    public IReadOnlyList<Drone> Drones => _drones.AsReadOnly();

    public void HandleDroneTurned(DroneTurned droneTurnedEvent)
    {
        var drone = _drones.Find(d => d.DroneId == droneTurnedEvent.DroneId);
        if (drone != null)
        {
            drone.Heading = droneTurnedEvent.Heading;
        }
    }

    public void AddDrone(DroneDropped drone)
    {
        _drones.Add(drone.Drone);
    }
}