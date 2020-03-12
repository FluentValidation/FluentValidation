namespace FluentValidation.Tests {
	using Xunit;

	public class MaxFailCountTest {
		private PersonValidator _validator = new PersonValidator();

		[Theory]
		[InlineData(1, 1)]
		[InlineData(2, 2)]
		[InlineData(3, 3)]
		[InlineData(4, 3)]
		[InlineData(5, 3)]
		public void Has_Errors(int failCount, int shouldBeCount) {
			var person = new Person();
			_validator.MaxFailCount = failCount;

			_validator.Validate(person).Errors.Count.ShouldEqual(shouldBeCount);
		}

		class Person {
			public int Age { get; set; }
			public string FirstName { get; set; }
			public string MiddleName { get; set; }
			public string LastName { get; set; }
			public char Sex { get; set; }

			public Person() {
			}

			public Person(int age, string firstName, string middleName, string lastName, char sex) {
				Age = age;
				FirstName = firstName;
				MiddleName = middleName;
				LastName = lastName;
				Sex = sex;
			}
		}

		class PersonValidator : AbstractValidator<Person> {
			public PersonValidator() {
				CascadeMode = CascadeMode.StopOnFirstFailure;

				RuleFor(s => s.Age).LessThanOrEqualTo(120).GreaterThan(0);
				RuleFor(s => s.FirstName).NotEmpty();
				RuleFor(s => s.Sex).Must(s => s == 'M' || s == 'F');
			}
		}
	}
}
