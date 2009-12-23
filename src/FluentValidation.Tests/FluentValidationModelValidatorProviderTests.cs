namespace FluentValidation.Tests {
	using System.Linq;
	using System.Web.Mvc;
	using Attributes;
	using Mvc;
	using NUnit.Framework;

	[TestFixture]
	public class FluentValidationModelValidatorProviderTests {
		ModelValidatorProvider provider;

		[SetUp]
		public void Setup() {
			provider = new FluentValidationModelValidatorProvider(new AttributedValidatorFactory());
		}

		[Test]
		public void Creates_modelvalidator_for_type() {
			var metaData = new ModelMetadata(new EmptyModelMetadataProvider(), null, null, typeof(Person), null);
			var validators = provider.GetValidators(metaData, new ControllerContext());

			validators.Single().ShouldBe<FluentValidationModelValidator>();
		}

		[Test]
		public void Performs_validation() {
			var metaData = new ModelMetadata(new EmptyModelMetadataProvider(), null, null, typeof(Person), null) {
				Model = new Person()
			};

			var validator = provider.GetValidators(metaData, new ControllerContext())
				.Single();

			var results = validator.Validate(null /*container*/);

			results.Count().ShouldEqual(1);
			results.Single().MemberName.ShouldEqual("Name");
			results.Single().Message.ShouldNotBeNull();
		}

		[Validator(typeof(PersonValidator))]
		private class Person {
			public string Name { get; set; }
		}

		private class PersonValidator : AbstractValidator<Person> {
			public PersonValidator() {
				RuleFor(x => x.Name).NotNull();
			}
		}
	}
}