namespace Ozon.Route256.Five.OrderService.Core.Handlers.PreOrders;

public interface IDistanceValidator
{
    bool IsValid(double lat1, double lon1, double lat2, double lon2);
}