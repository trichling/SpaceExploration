namespace SpaceExploration.Game.Planets;

public record Angle(int Degrees)
{
    public double Radians => Degrees * Math.PI / 180;

}
