namespace Ozon.Route256.Five.OrderService.Core.BusinessObjects;

public class CustomerBo
{
    public int Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string MobileNumber { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public AddressBo? DefaultAddress { get; init; }
    public AddressBo[]? Addresses { get; init; }
}
