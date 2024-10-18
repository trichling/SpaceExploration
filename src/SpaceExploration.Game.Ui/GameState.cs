namespace SpaceExploration.Game.Ui;

using SpaceExploration.Game.Contracts.Drones.Events;
using SpaceExploration.Game.Contracts.Planets.Events;
using SpaceExploration.Game.Planets;

public class GameState
{

    public event EventHandler StateChanged;

    public List<DroneScore> Scores { get; }
    public List<Drone> Drones { get; }
    public List<DroneShot> Shots { get; }
    public Guid WorldId { get; }

    public GameState(Guid worldId)
    {
        Drones = [];
        Shots = [];
        Scores = [];
        WorldId = worldId;
    }

    private GameState(Guid worldId, List<Drone> drones, List<DroneShot> shots, List<DroneScore> scores)
    {
        WorldId = worldId;
        Drones = drones;
        Shots = shots;
        Scores = scores;
    }

    internal void HandleCatchUpResponse(Contracts.Planets.Messages.CatchUpResponse message)
    {
        Drones.Clear();
        Shots.Clear();
        Scores.Clear();
        Drones.AddRange(message.Drones.Select(d => new Drone(d.DroneId, d.DroneType, d.DroneName, new Coordinate(d.X, d.Y), new Angle(d.Heading), d.Health)));
        Scores.AddRange(message.Drones.Select(s => new DroneScore(droneId: s.DroneId, droneName: s.DroneName, score: 0)));
        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void HandleDroneDropped(Contracts.Planets.Events.DroneDropped message)
    {

        Console.WriteLine(Drones.Count);
        if (!Drones.Exists(d => d.DroneId == message.DroneId))
        {
            Drones.Add(new Drone(message.DroneId, new Coordinate(message.X, message.Y), new Angle(message.Heading)));
            Scores.Add(new DroneScore(message.DroneId, message.DroneName, 0));
        }

        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void HandleDroneTurned(DroneTurned message)
    {

        if (!Drones.Exists(d => d.DroneId == message.DroneSignature))
        {
            var newDrone = new Drone(message.DroneSignature)
            {
                Position = new Coordinate(message.X, message.Y),
                Heading = new Angle(message.Heading)
            };
            Drones.Add(newDrone);
        }

        var drone = Drones.Find(d => d.DroneId == message.DroneSignature);
        if (drone != null)
        {
            drone.Position = new Coordinate(message.X, message.Y);
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
                Position = new Coordinate(message.X, message.Y),
                Heading = new Angle(message.Heading)
            };
            Drones.Add(newDrone);
        }

        var drone = Drones.Find(d => d.DroneId == message.DroneId);
        if (drone != null)
        {
            drone.Position = new Coordinate(message.X, message.Y);
            drone.Heading = new Angle(message.Heading);
        }

        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    internal void HandleDroneHit(DroneHit message)
    {

        if (!Drones.Exists(d => d.DroneId == message.ShootingDroneSignature))
        {
            var newDrone = new Drone(message.ShootingDroneSignature);
            Drones.Add(newDrone);
        }

        if (!Drones.Exists(d => d.DroneId == message.TargetDroneSignature))
        {
            var newDrone = new Drone(message.TargetDroneSignature);
            Drones.Add(newDrone);
        }

        var targetDrone = Drones.Find(d => d.DroneId == message.TargetDroneSignature);
        if (targetDrone != null)
        {
            targetDrone.Health = message.RemainingHealth;
        }

        var shootingDrone = Drones.Find(d => d.DroneId == message.ShootingDroneSignature);
        var shooterScore = Scores.Find(s => s.DroneId == message.ShootingDroneSignature);
        shooterScore.Score += 1;

        Shots.Add(new DroneShot { Shooter = shootingDrone, Target = targetDrone, Cycles = 0 });

        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    internal void HandleDroneDestroyed(DroneDestroyed message)
    {
        var drone = Drones.Find(d => d.DroneId == message.DroneSignature);
        if (drone != null)
        {
            Drones.Remove(drone);
        }

        var shooterDroneScore = Scores.Find(s => s.DroneId == message.DestroyedByDroneSignature);
        if (shooterDroneScore != null)
            shooterDroneScore.Score += 10;

        var destroyedDroneScore = Scores.Find(s => s.DroneId == message.DroneSignature);
        if (destroyedDroneScore != null)
            destroyedDroneScore.Score -= 10;

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
            Shots.ToList(),
            Scores.ToList()
        );
    }


}

public class DroneScore(Guid droneId, string droneName, int score)
{
    public Guid DroneId { get; } = droneId;
    public string DroneName { get; } = droneName;
    public int Score { get; set; } = score;
}
