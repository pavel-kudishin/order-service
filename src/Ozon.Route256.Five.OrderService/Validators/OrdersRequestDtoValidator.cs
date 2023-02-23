using FluentValidation;
using Ozon.Route256.Five.OrderService.Dto;
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