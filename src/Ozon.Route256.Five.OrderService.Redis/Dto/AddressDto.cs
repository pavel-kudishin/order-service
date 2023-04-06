﻿namespace Ozon.Route256.Five.OrderService.Redis.Dto;

internal record AddressDto(
    string City,
    string Street,
    string Building,
    string Apartment,
    double Latitude,
    double Longitude,
    string Region);