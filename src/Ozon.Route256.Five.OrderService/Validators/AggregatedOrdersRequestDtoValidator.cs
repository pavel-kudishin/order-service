﻿using FluentValidation;
using Ozon.Route256.Five.OrderService.Dto;
using System;

namespace Ozon.Route256.Five.OrderService.Validators;

internal sealed class AggregatedOrdersRequestDtoValidator
	: AbstractValidator<AggregatedOrdersRequestDto>
{
	public AggregatedOrdersRequestDtoValidator()
	{
		RuleFor(x => x.StartDate).Custom((startDate, context) =>
		{
			if (context.InstanceToValidate.EndDate.HasValue)
			{
				if (startDate > context.InstanceToValidate.EndDate)
				{
					context.AddFailure($"{nameof(OrdersByCustomerRequestDto.StartDate)} больше чем {nameof(OrdersByCustomerRequestDto.EndDate)}.");
				}
			}
		});
	}
}