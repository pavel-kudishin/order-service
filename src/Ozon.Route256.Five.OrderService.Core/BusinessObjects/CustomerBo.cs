namespace Ozon.Route256.Five.OrderService.Core.BusinessObjects;

public class CustomerBo
{
    public int Id { get; init; }
    public string FirstName { get; init; } = "";
    public string LastName { get; init; } = "";
    public string MobileNumber { get; init; } = "";
    public string Email { get; init; } = "";
    public AddressBo? Address { get; init; }
    public AddressBo[]? Addresses { get; init; }
}
