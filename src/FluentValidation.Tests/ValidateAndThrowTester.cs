namespace FluentValidation.Tests {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Internal;
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

			typeof(ValidationException).ShouldBeThrownBy(() => validator.ValidateAndThrow(new Person()));
		}

		[Fact]
		public void Throws_exception_with_a_ruleset() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull()
			};

			const string ruleSetName = "blah";
			validator.RuleSet(ruleSetName, () => { validator.RuleFor(x => x.Forename).NotNull(); });

			typeof(ValidationException).ShouldBeThrownBy(() => validator.ValidateAndThrow(new Person(), ruleSetName));
		}

		[Fact]
		public void Throws_exception_async() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull()
			};

			typeof(ValidationException).ShouldBeThrownBy(() => {
				try {
					validator.ValidateAndThrowAsync(new Person()).Wait();
				}
				catch (AggregateException agrEx) {
					throw agrEx.InnerException;
				}
			});
		}

		[Fact]
		public void Throws_exception_with_a_ruleset_async() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull()
			};

			const string ruleSetName = "blah";
			validator.RuleSet(ruleSetName, () => { validator.RuleFor(x => x.Forename).NotNull(); });

			typeof(ValidationException).ShouldBeThrownBy(() => {
				try {
					validator.ValidateAndThrowAsync(new Person(), ruleSetName).Wait();
				}
				catch (AggregateException agrEx) {
					throw agrEx.InnerException;
				}
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
			validator.ValidateAndThrow(person, ruleSetName);
		}

		[Fact]
		public void Does_not_throw_when_valid_async() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull()
			};

			validator.ValidateAndThrowAsync(new Person {Surname = "foo"}).Wait();
		}

		[Fact]
		public void Does_not_throw_when_valid_and_a_ruleset_async() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull()
			};

			const string ruleSetName = "blah";
			validator.RuleSet(ruleSetName, () => { validator.RuleFor(x => x.Forename).NotNull(); });

			var person = new Person {
				Forename = "foo",
				Surname = "foo"
			};
			validator.ValidateAndThrowAsync(person, ruleSetName).Wait();
		}

		[Fact]
		public void Populates_errors() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull()
			};

			var ex = (ValidationException) typeof(ValidationException).ShouldBeThrownBy(() => validator.ValidateAndThrow(new Person()));
			ex.Errors.Count().ShouldEqual(1);
		}

		[Fact]
		public void ToString_provides_error_details() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull(),
				v => v.RuleFor(x => x.Forename).NotNull()
			};

			var ex = typeof(ValidationException).ShouldBeThrownBy(() => validator.ValidateAndThrow(new Person()));
			string expected = "FluentValidation.ValidationException: Validation failed: " + Environment.NewLine + " -- Surname: 'Surname' must not be empty." + Environment.NewLine + " -- Forename: 'Forename' must not be empty.";
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
	}
}
