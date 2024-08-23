namespace SpaceExploration.Game.Planets;

public class Drone
{

    public Drone()
    {
        
    }

    public Drone(Guid droneId)
    {
        DroneId = droneId;
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

    public Guid DroneId { get; set; } 

    public readonly int FieldOvView = 45;
    public readonly double ScanDistance = 0.1;
    public readonly double MoveDistance = 0.01;

    public int Health { get; set; } = 10;

    public Coordinate Position { get; set; } = new Coordinate(new Random().NextDouble(), new Random().NextDouble());

    public Angle Heading { get; set; } = new Angle(new Random().Next(0, 360));
}
