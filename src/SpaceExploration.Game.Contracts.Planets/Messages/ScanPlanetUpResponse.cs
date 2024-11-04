namespace SpaceExploration.Game.Contracts.Planets.Messages;
public record ScanPlanetResponse(Guid PlanetId, List<Drone> Drones);

public class Drone(Guid droneSignature, string droneType, string droneName, double x, double y, int heading, int health)
{
    public Guid DroneSignature { get; set; } = droneSignature;
    public string DroneType { get; set; } = droneType;
    public string DroneName { get; set; } = droneName;
    public double X { get; set; } = x;
    public double Y { get; set; } = y;
    public int Heading { get; set; } = heading;
    public int Health { get; set; } = health;
}