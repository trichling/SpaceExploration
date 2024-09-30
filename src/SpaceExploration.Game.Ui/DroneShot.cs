namespace SpaceExploration.Game.Ui;

using SpaceExploration.Game.Planets;

public record DroneShot
{
    public Drone Shooter { get; set; }
    public Drone Target { get; set; }

    public int Cycles { get; set; }
}