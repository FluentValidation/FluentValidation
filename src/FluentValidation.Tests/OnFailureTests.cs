namespace FluentValidation.Tests {
	using System.Threading.Tasks;
	using System.Diagnostics;
	using Xunit;


	public class OnFailureTests {
		private TestValidator _validator;
		public OnFailureTests() {
			_validator = new TestValidator();
		}

		[Fact]
		public void on_failure_called_for_each_failed_rule() {
			_validator.CascadeMode = CascadeMode.Continue;

			int invoked = 0;
			_validator.RuleFor(person => person.Surname).NotNull().NotEmpty().OnFailure(person => {
				invoked += 1;
			});

			_validator.RuleFor(person => person.Surname).NotEmpty().OnFailure((person, ctx) => {
				Debug.WriteLine(ctx.PropertyName);
				invoked += 1;
			});

			_validator.RuleFor(person => person.Forename).NotEqual("John").OnFailure((person, ctx) => {
				invoked += 1;
			});

			_validator.RuleFor(person => person.Age).GreaterThanOrEqualTo(18).OnFailure((person, ctx) => {
				invoked += 1;
			});

			_validator.Validate(new Person { Forename = "John", Age = 17 });

			invoked.ShouldEqual(4);
		}

		[Fact]
		public async Task on_failure_called_for_each_failed_rule_asyncAsync() {
			_validator.CascadeMode = CascadeMode.Continue;

			int invoked = 0;
			_validator.RuleFor(person => person.Surname).NotNull().NotEmpty().OnFailure(person => {
				invoked += 1;
			});

			_validator.RuleFor(person => person.Surname).NotEmpty().OnFailure((person, ctx) => {
				Debug.WriteLine(ctx.PropertyName);
				invoked += 1;
			});

			_validator.RuleFor(person => person.Forename).NotEqual("John").OnFailure((person, ctx) => {
				invoked += 1;
			});

			_validator.RuleFor(person => person.Age).GreaterThanOrEqualTo(18).OnFailure((person, ctx) => {
				invoked += 1;
			});

			await _validator.ValidateAsync(new Person { Forename = "John", Age = 17 });

			invoked.ShouldEqual(4);
		}

		[Fact]
		public void should_be_able_to_access_error_message_in_on_failure() {
			_validator.CascadeMode = CascadeMode.Continue;

			string errorMessage = string.Empty;
			_validator.RuleFor(person => person.Surname).NotNull().WithMessage("You must specify Surname!").OnFailure((person,ctx, message) => {
				errorMessage = message;
			});


			_validator.Validate(new Person());

			errorMessage.ShouldEqual("You must specify Surname!");
		}
	}
}