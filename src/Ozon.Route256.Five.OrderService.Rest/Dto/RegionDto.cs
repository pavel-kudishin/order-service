﻿using System.ComponentModel.DataAnnotations;

namespace Ozon.Route256.Five.OrderService.Rest.Dto;

public sealed class RegionDto
{
    [Required]
    public string? Name { get; init; }

    public WarehouseDto? Warehouse { get; init; }
}