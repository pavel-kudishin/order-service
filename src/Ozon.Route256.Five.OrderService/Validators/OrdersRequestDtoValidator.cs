using FluentValidation;
using Ozon.Route256.Five.OrderService.Rest.Dto;
using System;

namespace Ozon.Route256.Five.OrderService.Validators;

internal sealed class OrdersRequestDtoValidator
	: AbstractValidator<OrdersRequestDto>
{
	public OrdersRequestDtoValidator()
	{
		// TODO
	}
}