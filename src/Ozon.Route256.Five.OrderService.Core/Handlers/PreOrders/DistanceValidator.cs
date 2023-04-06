namespace Ozon.Route256.Five.OrderService.Core.Handlers.PreOrders;

public sealed class DistanceValidator : IDistanceValidator
{
    public bool IsValid(double lat1, double lon1, double lat2, double lon2)
    {
        double distance = Distance(lat1, lon1, lat2, lon2);

        const double MAX_DISTANCE_KM = 5_000d;

        return distance <= MAX_DISTANCE_KM;
    }

    private static double Distance(double lat1, double lon1, double lat2, double lon2)
    {
        double lat1Radians = DegreesToRadians(lat1);
        double lat2Radians = DegreesToRadians(lat2);
        double thetaRadians = DegreesToRadians(lon1 - lon2);
        double dist = Math.Sin(lat1Radians) * Math.Sin(lat2Radians) +
                      Math.Cos(lat1Radians) * Math.Cos(lat2Radians) * Math.Cos(thetaRadians);
        dist = Math.Acos(dist);
        dist = RadiansToDegrees(dist) * 60 * 1.1515 * 1.609344;
        return dist;

        double DegreesToRadians(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        double RadiansToDegrees(double rad)
        {
            return (rad / Math.PI * 180.0);
        }
    }
}