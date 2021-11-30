namespace FluentValidation.Tests {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Newtonsoft.Json;
	using Results;
	using Xunit;

	public class ValidateAndThrowTester {
		public ValidateAndThrowTester() {
			CultureScope.SetDefaultCulture();
		}

		[Fact]
		public void Throws_exception() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull()
			};

			Assert.Throws<ValidationException>(() => validator.ValidateAndThrow(new Person()));
		}

		[Fact]
		public void Throws_exception_with_a_ruleset() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull()
			};

			const string ruleSetName = "blah";
			validator.RuleSet(ruleSetName, () => { validator.RuleFor(x => x.Forename).NotNull(); });

			Assert.Throws<ValidationException>(() => {
				validator.Validate(new Person(), v => v.IncludeRuleSets(ruleSetName).ThrowOnFailures());
			});
		}

		[Fact]
		public async Task Throws_exception_async() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull()
			};

			await Assert.ThrowsAsync<ValidationException>(async () => {
				await validator.ValidateAndThrowAsync(new Person());
			});
		}

		[Fact]
		public async Task Throws_exception_with_a_ruleset_async() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull()
			};

			const string ruleSetName = "blah";
			validator.RuleSet(ruleSetName, () => { validator.RuleFor(x => x.Forename).NotNull(); });

			await Assert.ThrowsAsync<ValidationException>(async () => {
				await validator.ValidateAsync(new Person(), v => v.IncludeRuleSets(ruleSetName).ThrowOnFailures());
			});
		}

		[Fact]
		public void Does_not_throw_when_valid() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull()
			};

			validator.ValidateAndThrow(new Person {Surname = "foo"});
		}

		[Fact]
		public void Does_not_throw_when_valid_and_a_ruleset() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull()
			};

			const string ruleSetName = "blah";
			validator.RuleSet(ruleSetName, () => { validator.RuleFor(x => x.Forename).NotNull(); });

			var person = new Person {
				Forename = "foo",
				Surname = "foo"
			};
			validator.Validate(person, v => v.IncludeRuleSets(ruleSetName).ThrowOnFailures());
		}

		[Fact]
		public async Task Does_not_throw_when_valid_async() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull()
			};

			await validator.ValidateAndThrowAsync(new Person {Surname = "foo"});
		}

		[Fact]
		public async Task Does_not_throw_when_valid_and_a_ruleset_async() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull()
			};

			const string ruleSetName = "blah";
			validator.RuleSet(ruleSetName, () => { validator.RuleFor(x => x.Forename).NotNull(); });

			var person = new Person {
				Forename = "foo",
				Surname = "foo"
			};
			await validator.ValidateAsync(person, v => v.IncludeRuleSets(ruleSetName).ThrowOnFailures());
		}

		[Fact]
		public void Populates_errors() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull()
			};

			var ex = (ValidationException) Assert.Throws<ValidationException>(() => validator.ValidateAndThrow(new Person()));
			ex.Errors.Count().ShouldEqual(1);
		}

		[Fact]
		public void ToString_provides_error_details() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull(),
				v => v.RuleFor(x => x.Forename).NotNull()
			};

			var ex = Assert.Throws<ValidationException>(() => validator.ValidateAndThrow(new Person()));
			string expected = "FluentValidation.ValidationException: Validation failed: " + Environment.NewLine + " -- Surname: 'Surname' must not be empty. Severity: Error" + Environment.NewLine + " -- Forename: 'Forename' must not be empty.";
			Assert.True(ex.ToString().StartsWith(expected));
		}

		[Fact]
		public void Serializes_exception() {
			var v = new ValidationException(new List<ValidationFailure> {new ValidationFailure("test", "test")});
			var raw = JsonConvert.SerializeObject(v);
			var deserialized = JsonConvert.DeserializeObject<ValidationException>(raw);

			deserialized.Errors.Count().ShouldEqual(1);
		}

		[Fact]
		public void ValidationException_provides_correct_message_when_appendDefaultMessage_true() {
			var userMessage = "exception occured during testing";
			var validationFailures = new List<ValidationFailure> {new ValidationFailure("test", "test")};
			var exception = new ValidationException(validationFailures);
			var exceptionWithUserMessage = new ValidationException(userMessage, validationFailures, true);

			exceptionWithUserMessage.Message.ShouldEqual($"{userMessage} {exception.Message}");
		}

		[Fact]
		public void ValidationException_provides_correct_message_when_appendDefaultMessage_false() {
			var userMessage = "exception occured during testing";
			var validationFailures = new List<ValidationFailure> {new ValidationFailure("test", "test")};
			var exceptionWithUserMessage = new ValidationException(userMessage, validationFailures, false);

			exceptionWithUserMessage.Message.ShouldEqual(userMessage);
		}

		[Fact]
		public void Only_root_validator_throws() {
			var validator = new InlineValidator<Person>();
			var addressValidator = new InlineValidator<Address>();
			validator.RuleFor(x => x.Address).SetValidator(addressValidator);
			validator.RuleFor(x => x.Forename).NotNull();
			addressValidator.RuleFor(x => x.Line1).NotNull();

			bool thrown = false;

			// Child validator shouldn't throw the exception, only the root validator should.
			// If the child validator threw the exception, then there would only be 1 failure in the validation result.
			// But if the root is throwing, then there should be 2 (as it collected its own failure and the child failure).
			try {
				validator.ValidateAndThrow(new Person() {Address = new Address()});
			}
			catch (ValidationException e) {
				e.Errors.Count().ShouldEqual(2);
				thrown = true;
			}

			thrown.ShouldBeTrue();
		}

		[Fact]
		public void Throws_exception_when_preValidate_fails_and_continueValidation_true() {
			var validator = new TestValidatorWithPreValidate {
				PreValidateMethod = (context, result) => {
					result.Errors.Add(new ValidationFailure("test", "test"));
					return true;
				}
			};

			Assert.Throws<ValidationException>(() => validator.ValidateAndThrow(new Person()));
		}

		[Fact]
		public void Throws_exception_when_preValidate_fails_and_continueValidation_false() {
			var validator = new TestValidatorWithPreValidate {
				PreValidateMethod = (context, result) => {
					result.Errors.Add(new ValidationFailure("test", "test"));
					return false;
				}
			};

			Assert.Throws<ValidationException>(() => validator.ValidateAndThrow(new Person()));
		}

		[Fact]
		public void Does_not_throws_exception_when_preValidate_ends_with_continueValidation_false() {
			var validator = new TestValidatorWithPreValidate {
				PreValidateMethod = (context, result) => false
			};

			validator.ValidateAndThrow(new Person());
		}

		[Fact]
		public async Task Throws_exception_when_preValidate_fails_and_continueValidation_true_async() {
			var validator = new TestValidatorWithPreValidate {
				PreValidateMethod = (context, result) => {
					result.Errors.Add(new ValidationFailure("test", "test"));
					return true;
				}
			};

			await Assert.ThrowsAsync<ValidationException>(async () => {
				await validator.ValidateAndThrowAsync(new Person());
			});
		}

		[Fact]
		public async Task Throws_exception_when_preValidate_fails_and_continueValidation_false_async() {
			var validator = new TestValidatorWithPreValidate {
				PreValidateMethod = (context, result) => {
					result.Errors.Add(new ValidationFailure("test", "test"));
					return false;
				}
			};

			await Assert.ThrowsAsync<ValidationException>(async () => {
				await validator.ValidateAndThrowAsync(new Person());
			});
		}

		[Fact]
		public async Task Does_not_throws_exception_when_preValidate_ends_with_continueValidation_false_async() {
			var validator = new TestValidatorWithPreValidate {
				PreValidateMethod = (context, result) => false
			};

			await validator.ValidateAndThrowAsync(new Person());
		}

		[Fact]
		public void Throws_when_calling_validator_as_interface() {
			IValidator<TestType> validator = new InterfaceValidator();
			var test = new TestType {
				TestInt = 0
			};
			Assert.Throws<ValidationException>(() => validator.ValidateAndThrow(test));
		}

		public class InterfaceValidator : AbstractValidator<ITestType> {
			public InterfaceValidator() {
				RuleFor(t => t.TestInt).GreaterThanOrEqualTo(1);
			}
		}

		public class TestType : ITestType {
			public int TestInt { get; set; }
		}

		public interface ITestType {
			int TestInt { get; set; }
		}
	}
}
