namespace SpaceExploration.Game.Ui;

using SpaceExploration.Game.Planets;

public record DroneShot
{
    public Drone Shooter { get; set; }
    public Drone Target { get; set; }

    public bool Drawn { get; set; }
}