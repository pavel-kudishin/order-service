using FluentValidation;
using Ozon.Route256.Five.OrderService.Rest.Dto;

namespace Ozon.Route256.Five.OrderService.Rest.Validators;

internal sealed class OrdersByCustomerRequestDtoValidator
	: AbstractValidator<OrdersByCustomerRequestDto>
{
	public OrdersByCustomerRequestDtoValidator()
	{
		RuleFor(x => x.CustomerId).GreaterThanOrEqualTo(0);
		RuleFor(x => x.StartDate).Custom((startDate, context) =>
		{
			if (startDate.HasValue && context.InstanceToValidate.EndDate.HasValue)
			{
				if (startDate > context.InstanceToValidate.EndDate)
				{
					context.AddFailure($"{nameof(OrdersByCustomerRequestDto.StartDate)} больше чем {nameof(OrdersByCustomerRequestDto.EndDate)}.");
				}
			}
		});
	}
}