namespace SpaceExploration.Game.Planets;

public class Drone
{

    public Drone()
    {

    }

    public Drone(Guid droneId)
    {
        DroneId = droneId;
        DroneType = "Default";
        DroneName = $"{DroneType}-{droneId.ToString()}";
    }

    public Drone(Guid droneId, string droneType)
    {
        DroneId = droneId;
        DroneType = droneType;
        DroneName = $"{droneType}-{droneId.ToString()}";
    }

    public Drone(Guid droneId, string droneType, string droneName)
    {
        DroneId = droneId;
        DroneType = droneType;
        DroneName = droneName;
    }

    public Drone(Guid droneId, Coordinate position, Angle heading) : this(droneId)
    {
        Position = position;
        Heading = heading;
    }

    public Drone(Guid droneId, Coordinate position, Angle heading, int health) : this(droneId)
    {
        Position = position;
        Heading = heading;
        Health = health;
    }

    public Drone(Guid droneId, string droneType, string droneName, Coordinate position, Angle heading, int health) : this(droneId)
    {
        DroneType = droneType;
        DroneName = droneName;
        Position = position;
        Heading = heading;
        Health = health;
    }

    public Guid DroneId { get; set; }
    public string DroneType { get; set; }
    public string DroneName { get; set; }

    public readonly int FieldOvView = 45;
    public readonly double ScanDistance = 0.1;
    public readonly double MoveDistance = 0.01;

    public int Health { get; set; } = 10;

    public Coordinate Position { get; set; } = new Coordinate(new Random().NextDouble(), new Random().NextDouble());

    public Angle Heading { get; set; } = new Angle(new Random().Next(0, 360));
}
