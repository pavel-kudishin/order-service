﻿namespace Ozon.Route256.Five.OrderService.Core.BusinessObjects;

public class OrderBo
{
    public long Id { get; init; }
    public int GoodsCount { get; init; }
    public decimal TotalPrice { get; init; }
    public decimal TotalWeight { get; init; }
    public OrderSourceBo Source { get; init; }
    public DateTimeOffset DateCreated { get; init; }
    public OrderStateBo State { get; init; }
    public CustomerBo? Customer { get; init; }
    public AddressBo? Address { get; init; }
    public string Phone { get; init; } = string.Empty;
}