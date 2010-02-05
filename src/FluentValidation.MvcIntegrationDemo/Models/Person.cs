namespace FluentValidation.MvcIntegrationDemo.Models {
	using Attributes;

	[Validator(typeof(PersonValidator))]
	public class Person {
		public int Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public int Age { get; set; }
	}

	public class PersonValidator : AbstractValidator<Person> {
		public PersonValidator() {
			RuleFor(x => x.Id).NotNull();
			RuleFor(x => x.Name).Length(0, 10);
			RuleFor(x => x.Email).EmailAddress();
			RuleFor(x => x.Age).InclusiveBetween(18, 60);
		}
	}
}