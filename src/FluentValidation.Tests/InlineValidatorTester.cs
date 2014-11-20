namespace FluentValidation.Tests {
	using Xunit;
	using Results;

	
	public class InlineValidatorTester {
		[Fact]
		public void Uses_inline_validator_to_build_rules() {
			var cust = new Customer();
			var result = cust.Validate();

			result.Errors.Count.ShouldEqual(2);
		}

		public class Customer {
			public int Id { get; set; }
			public string Name { get; set; }

			public ValidationResult Validate() {
				return Validator.Validate(this);
			}

			public static readonly InlineValidator<Customer> Validator = new InlineValidator<Customer>() {
				v => v.RuleFor(x => x.Name).NotNull(),
				v => v.RuleFor(x => x.Id).NotEqual(0)
			};
		}
	}
}