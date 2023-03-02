using FluentAssertions;
using FluentValidation.Results;
using Ozon.Route256.Five.OrderService.Dto;
using Ozon.Route256.Five.OrderService.Validators;

namespace Ozon.Route256.Five.OrderService.Tests;

public class OrdersByCustomerRequestDtoValidatorTests
{
	private readonly OrdersByCustomerRequestDtoValidator _validator;

	public OrdersByCustomerRequestDtoValidatorTests()
	{
		_validator = new OrdersByCustomerRequestDtoValidator();
	}

	[Fact]
	public void Validate_Successful()
	{
		OrdersByCustomerRequestDto request = new()
		{
			CustomerId = 1,
		};
		ValidationResult validationResult = _validator.Validate(request);
		validationResult.IsValid.Should().BeTrue();
	}

	[Fact]
	public void Validate_Successful1()
	{
		OrdersByCustomerRequestDto request = new()
		{
			CustomerId = 1,
			StartDate = DateTime.Now.AddDays(-10),
			EndDate = DateTime.Now.AddDays(-3)
		};
		ValidationResult validationResult = _validator.Validate(request);
		validationResult.IsValid.Should().BeTrue();
	}

	[Fact]
	public void Validate_Successful2()
	{
		OrdersByCustomerRequestDto request = new()
		{
			CustomerId = 1,
			StartDate = DateTime.Now.AddDays(-10),
		};
		ValidationResult validationResult = _validator.Validate(request);
		validationResult.IsValid.Should().BeTrue();
	}

	[Fact]
	public void Validate_Failed()
	{
		OrdersByCustomerRequestDto request = new()
		{
			CustomerId = 1,
			StartDate = DateTime.Now.AddDays(-1),
			EndDate = DateTime.Now.AddDays(-3)
		};
		ValidationResult validationResult = _validator.Validate(request);
		validationResult.IsValid.Should().BeFalse();
	}
}