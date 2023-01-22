namespace FluentValidation.Tests;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Internal;
using Results;
using Xunit;

public class MemberNameValidatorTester {
	private readonly MemberNamePersonValidator _validator;
	private readonly Person _person;
	public MemberNameValidatorTester() {
		_validator = new MemberNamePersonValidator();
		_person = new Person {
			Address = new Address {
				Country = new Country()
			},
			Orders = new List<Order> {
				new() {Amount = 5},
				new() {ProductName = "Foo"}
			}
		};
	}
	[Fact]
	public void Path_to_country_should_validate_only_single_property() {
		var validatorSelector = new MemberNameValidatorSelector(new[] { "Address.Country.Name" });
		var fluentValidationContext = new ValidationContext<Person>(_person, new PropertyChain(), validatorSelector);
		ValidationResult result = _validator.Validate(fluentValidationContext);
		result.Errors.Count.ShouldEqual(1);
		result.Errors.Single().ErrorMessage.ShouldEqual("'Address Country Name' must not be empty.");
	}

	[Fact]
	public void Path_to_index_should_validate_single_list_item()
	{
		var validatorSelector = new MemberNameValidatorSelector(new[] { "Orders[1].Amount" });
		var fluentValidationContext = new ValidationContext<Person>(_person, new PropertyChain(), validatorSelector);
		ValidationResult result = _validator.Validate(fluentValidationContext);
		result.Errors.Count.ShouldEqual(1);
		result.Errors.Single().ErrorMessage.ShouldEqual("'Amount' must be greater than '6'.");
	}
	[Fact]
	public void Path_to_list_should_validate_all_list_items()
	{
		var validatorSelector = new MemberNameValidatorSelector(new[] { "Orders[].Amount" });
		var fluentValidationContext = new ValidationContext<Person>(_person, new PropertyChain(), validatorSelector);
		ValidationResult result = _validator.Validate(fluentValidationContext);
		result.Errors.Count.ShouldEqual(2);
		result.Errors[0].ErrorMessage.ShouldEqual("'Amount' must be greater than '6'.");
		result.Errors[1].ErrorMessage.ShouldEqual("'Amount' must be greater than '6'.");
	}
	private class MemberNamePersonValidator : AbstractValidator<Person> {
		public MemberNamePersonValidator() {

			RuleFor(x => x.Surname).NotEmpty();
			RuleFor(x => x.Address.Country.Name).NotEmpty();
			RuleForEach(x => x.Orders).ChildRules(x => {
				x.RuleFor(y => y.Amount).GreaterThan(6);
				x.RuleFor(y => y.ProductName).MinimumLength(5);
			});

		}

	}
}
