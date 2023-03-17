﻿namespace Ozon.Route256.Five.OrderService.Kafka.Consumers.PreOrders;

public record PreOrderDto(
    long Id,
    PreOrderSource Source,
    PreOrderCustomerDto Customer,
    IReadOnlyList<PreOrderGoodsDto> Goods);