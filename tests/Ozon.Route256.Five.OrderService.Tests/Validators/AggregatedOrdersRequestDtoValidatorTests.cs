using FluentAssertions;
using FluentValidation.Results;
using Ozon.Route256.Five.OrderService.Rest.Dto;
using Ozon.Route256.Five.OrderService.Validators;

namespace Ozon.Route256.Five.OrderService.Tests.Validators;

public class AggregatedOrdersRequestDtoValidatorTests
{
    private readonly AggregatedOrdersRequestDtoValidator _validator;

    public AggregatedOrdersRequestDtoValidatorTests()
    {
        _validator = new AggregatedOrdersRequestDtoValidator();
    }

    [Fact]
    public void Validate_Successful()
    {
        AggregatedOrdersRequestDto request = new();
        ValidationResult validationResult = _validator.Validate(request);
        validationResult.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_Successful1()
    {
        AggregatedOrdersRequestDto request = new()
        {
            StartDate = DateTime.Now.AddDays(-10),
            EndDate = DateTime.Now.AddDays(-3)
        };
        ValidationResult validationResult = _validator.Validate(request);
        validationResult.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_Successful2()
    {
        AggregatedOrdersRequestDto request = new()
        {
            StartDate = DateTime.Now.AddDays(-10),
        };
        ValidationResult validationResult = _validator.Validate(request);
        validationResult.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_Failed()
    {
        AggregatedOrdersRequestDto request = new()
        {
            StartDate = DateTime.Now.AddDays(-1),
            EndDate = DateTime.Now.AddDays(-3)
        };
        ValidationResult validationResult = _validator.Validate(request);
        validationResult.IsValid.Should().BeFalse();
    }
}