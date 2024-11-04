namespace SpaceExploration.Game.Ui;

using SpaceExploration.Game.Contracts.Drones.Events;
using SpaceExploration.Game.Contracts.Planets.Events;
using SpaceExploration.Game.Contracts.Planets.Messages;

public class GameState
{

    public event EventHandler StateChanged;

    public List<Drone> Drones { get; }
    public List<DroneScore> Scores { get; }
    public List<DroneShot> Shots { get; }
    public Guid WorldId { get; }

    public GameState(Guid worldId)
    {
        Drones = [];
        Scores = [];
        Shots = [];
        WorldId = worldId;
    }

    private GameState(Guid worldId, List<Drone> drones, List<DroneScore> scores, List<DroneShot> shots)
    {
        WorldId = worldId;
        Drones = drones;
        Scores = scores;
        Shots = shots;
    }

    internal void HandleCatchUpResponse(ScanPlanetResponse message)
    {
        Drones.Clear();
        Shots.Clear();
        Drones.AddRange(message.Drones.Select(d => new Drone(d.DroneSignature, d.DroneType, d.DroneName, d.X, d.Y, d.Heading, d.Health)));
        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void HandleDroneDropped(Contracts.Planets.Events.DroneDropped message)
    {
        if (!Drones.Exists(d => d.DroneSignature == message.DroneSignature))
        {
            Drones.Add(new Drone(message.DroneSignature, message.DroneType, message.DroneName, message.X, message.Y, message.Heading, 10));
        }

        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void HandleDroneTurned(DroneTurned message)
    {
        var drone = Drones.Find(d => d.DroneSignature == message.DroneSignature);
        if (drone != null)
        {
            drone.X = message.X;
            drone.Y = message.Y;
            drone.Heading = message.Heading;
        }

        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    internal void HandleDroneMoved(DroneMoved message)
    {
        var drone = Drones.Find(d => d.DroneSignature == message.DroneSignature);
        if (drone != null)
        {
            drone.X = message.X;
            drone.Y = message.Y;
            drone.Heading = message.Heading;
        }

        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    internal void HandleDroneHit(DroneHit message)
    {
        var targetDrone = Drones.Find(d => d.DroneSignature == message.TargetDroneSignature);
        if (targetDrone != null)
        {
            targetDrone.Health = message.RemainingHealth;
        }

        var shootingDrone = Drones.Find(d => d.DroneSignature == message.ShootingDroneSignature);

        Shots.Add(new DroneShot { Shooter = shootingDrone, Target = targetDrone, Cycles = 0 });

        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    internal void HandleDroneDestroyed(DroneDestroyed message)
    {
        var drone = Drones.Find(d => d.DroneSignature == message.DroneSignature);
        if (drone != null)
        {
            Drones.Remove(drone);
        }

        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    internal void HandleDroneScoreUpdated(DroneScoreUpdated message)
    {

        var score = Scores.Find(d => d.DroneSignature == message.DroneSignature);
        if (score == null)
        {
            score = new DroneScore { DroneSignature = message.DroneSignature, Score = message.Score };
            Scores.Add(score);
        }

        score.Score = message.Score;

        var drone = Drones.Find(d => d.DroneSignature == message.DroneSignature);
        if (drone != null)
        {
            score.DroneName = drone.DroneName;
        }

        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    internal void RemoveDrawnShots()
    {
        foreach (var shot in Shots)
        {
            shot.Cycles++;
        }

        Shots.RemoveAll(s => s.Cycles > 3);
    }

    internal GameState Freeze()
    {
        return new GameState
        (
            WorldId,
            Drones.ToList(),
            Scores.ToList(),
            Shots.ToList()
        );
    }


}

public class DroneScore
{
    public Guid DroneSignature { get; set; }
    public string DroneName { get; set; }
    public int Score { get; set; }
}