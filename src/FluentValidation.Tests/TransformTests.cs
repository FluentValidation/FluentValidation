namespace FluentValidation.Tests {
	using Xunit;

	public class TransformTests {
		[Fact]
		public void Transforms_property_value() {
			var validator = new InlineValidator<Person>();
			validator.RuleFor(x => x.Surname).Transform(name => "foo" + name).Equal("foobar");

			var result = validator.Validate(new Person {Surname = "bar"});
			result.IsValid.ShouldBeTrue();
		}
	}
}