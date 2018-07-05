using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentValidation.Tests
{
	using System.Diagnostics;
	using Xunit;


	public class OnFailureTests
	{
		private readonly Xunit.Abstractions.ITestOutputHelper output;

		TestValidator validator;
		public OnFailureTests(Xunit.Abstractions.ITestOutputHelper output)
		{
			this.output = output;

			validator = new TestValidator();
		}

		[Fact]
		public void Invokes_twice_for_two_rules_on_failure()
		{
			validator.CascadeMode = CascadeMode.Continue;

			int invoked = 0;
			validator.RuleFor(person => person.Surname).NotNull().NotEmpty().OnFailure( person => {
				invoked += 1;
			});

			validator.RuleFor(person => person.Surname).NotEmpty().OnFailure((person,ctx) => {
				output.WriteLine(ctx.PropertyName);
				invoked += 1;
			});

			validator.RuleFor(person => person.Forename).NotEqual("John").OnFailure((person, ctx, message) =>
			{
				output.WriteLine(message);
				invoked += 1;
			}, "Can't be John");

			validator.RuleFor(person => person.Age).GreaterThanOrEqualTo(18).OnFailure((person, ctx, message) =>
			{
				output.WriteLine(message);
				invoked += 1;
			}, "You must be at least 18 years old. You entered: {0}", person=>person.Age);

			validator.Validate(new Person { Forename="John", Age = 17 });

			invoked.ShouldEqual(4);
		}

	}
}