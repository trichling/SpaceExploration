namespace SpaceExploration.Game.Planets;

public class EnvironmentalScan
{

    public static List<VisibleDrone> ScanEnvironment(Drone drone, List<Drone> drones)
    {
        var visibleDrones = new List<VisibleDrone>();

        foreach (var otherDrone in drones)
        {
            if (otherDrone != drone)
            {
                var sensorReading = IsDroneVisible(drone, otherDrone); 
                if (sensorReading != null)
                {
                    visibleDrones.Add(sensorReading);
                }
            }
        }

        return visibleDrones;
    }

    private static VisibleDrone? IsDroneVisible(Drone drone, Drone otherDrone)
    {
        double angleToOtherDrone = Math.Atan2(otherDrone.Position.Y - drone.Position.Y, otherDrone.Position.X - drone.Position.X) * 180 / Math.PI;
        double relativeAngle = (angleToOtherDrone - drone.Heading.Degrees + 360) % 360;

        if (relativeAngle > 180)
        {
            relativeAngle -= 360;
        }

        double distanceToOtherDrone = Math.Sqrt(Math.Pow(otherDrone.Position.X - drone.Position.X, 2) + Math.Pow(otherDrone.Position.Y - drone.Position.Y, 2));

        var isWithinViewport = Math.Abs(relativeAngle) <= drone.FieldOvView / 2 && distanceToOtherDrone <= drone.ScanDistance;

        if (isWithinViewport)
        {
            return new VisibleDrone(otherDrone, distanceToOtherDrone, relativeAngle);
        }

        return null;
    }

}


public record VisibleDrone(Drone Drone, double Distance, double Angle);
