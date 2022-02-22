namespace FluentValidation.Tests;

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using Xunit;
using TestHelper;

public class OnFailureTests {
	private TestValidator _validator;

	public OnFailureTests() {
		_validator = new TestValidator();
	}

	[Fact]
	public void OnFailure_called_for_each_failed_rule() {
		int invoked = 0;
		_validator.RuleFor(person => person.Surname).NotNull().NotEmpty().OnFailure(person => {
			invoked += 1;
		});

		_validator.RuleFor(person => person.Surname).NotEmpty().OnFailure((person, ctx, value) => {
			Debug.WriteLine(ctx.PropertyName);
			invoked += 1;
		});

		_validator.RuleFor(person => person.Forename).NotEqual("John").OnFailure((person, ctx, value) => {
			invoked += 1;
		});

		_validator.RuleFor(person => person.Age).GreaterThanOrEqualTo(18).OnFailure((person, ctx, value) => {
			invoked += 1;
		});

		_validator.Validate(new Person { Forename = "John", Age = 17 });

		invoked.ShouldEqual(4);
	}

	[Fact]
	public async Task OnFailure_called_for_each_failed_rule_asyncAsync() {
		int invoked = 0;
		_validator.RuleFor(person => person.Surname).NotNull().NotEmpty().OnFailure(person => {
			invoked += 1;
		});

		_validator.RuleFor(person => person.Surname).NotEmpty().OnFailure((person, ctx, value) => {
			Debug.WriteLine(ctx.PropertyName);
			invoked += 1;
		});

		_validator.RuleFor(person => person.Forename).NotEqual("John").OnFailure((person, ctx, value) => {
			invoked += 1;
		});

		_validator.RuleFor(person => person.Age).GreaterThanOrEqualTo(18).OnFailure((person, ctx, value) => {
			invoked += 1;
		});

		await _validator.ValidateAsync(new Person { Forename = "John", Age = 17 });

		invoked.ShouldEqual(4);
	}

	[Fact]
	public void Should_be_able_to_access_error_message_in_OnFailure() {
		string errorMessage = string.Empty;
		_validator.RuleFor(person => person.Surname).NotNull().WithMessage("You must specify Surname!").OnFailure((person,ctx, value, message) => {
			errorMessage = message;
		});


		_validator.Validate(new Person());

		errorMessage.ShouldEqual("You must specify Surname!");
	}

	[Fact]
	public void ShouldHaveChildValidator_should_be_true() {
		_validator.RuleFor(person => person.Address).SetValidator(new AddressValidatorWithOnFailure()).OnFailure((p,ctx, value)=> { Debug.WriteLine(p.Forename); });
		_validator.ShouldHaveChildValidator(x => x.Address, typeof(AddressValidatorWithOnFailure));
	}

	[Fact]
	public void ShouldHaveChildValidator_works_with_Include() {
		_validator.Include(new InlineValidator<Person>() {
			v => v.RuleFor(x => x.Forename).NotNull(),
		});

		_validator.ShouldHaveChildValidator(x => x, typeof(InlineValidator<Person>));
	}

	[Fact]
	public void WhenWithOnFailure_should_invoke_condition_on_inner_validator() {
		bool shouldNotBeTrue = false;
		var validator = new TestValidator();
		validator.RuleFor(x => x.Surname)
			.NotEqual("foo")
			.When(x => x.Id > 0)
			.OnFailure(x => shouldNotBeTrue = true);

		var result = validator.Validate(new Person {Id = 0, Surname = "foo"});
		result.Errors.Count.ShouldEqual(0);
		shouldNotBeTrue.ShouldBeFalse();
	}

	[Fact]
	public async Task WhenAsyncWithOnFailure_should_invoke_condition_on_inner_validator() {
		bool shouldNotBeTrue = false;
		var validator = new TestValidator();
		validator.RuleFor(x => x.Surname)
			.NotEqual("foo")
			.WhenAsync((x, token) => Task.FromResult(x.Id > 0))
			.OnFailure(x => shouldNotBeTrue = true);

		var result = await validator.ValidateAsync(new Person {Id = 0, Surname = "foo"});
		result.Errors.Count.ShouldEqual(0);
		shouldNotBeTrue.ShouldBeFalse();
	}

	[Fact]
	public async Task WhenWithOnFailure_should_invoke_condition_on_async_inner_validator() {
		bool shouldNotBeTrue = false;
		var validator = new TestValidator();
		validator.RuleFor(x => x.Surname)
			.MustAsync((x, ctx) => Task.FromResult(false))
			.When(x => x.Id > 0)
			.OnFailure(x => shouldNotBeTrue = true);

		var result = await validator.ValidateAsync(new Person {Id = 0, Surname = "foo"});
		result.Errors.Count.ShouldEqual(0);
		shouldNotBeTrue.ShouldBeFalse();
	}

	[Fact]
	public async Task WhenAsyncWithOnFailure_should_invoke_condition_on_async_inner_validator() {
		bool shouldNotBeTrue = false;
		var validator = new TestValidator();
		validator.RuleFor(x => x.Surname)
			.MustAsync((x, ctx) => Task.FromResult(false))
			.WhenAsync((x, ctx) => Task.FromResult(x.Id > 0))
			.OnFailure(x => shouldNotBeTrue = true);

		var result = await validator.ValidateAsync(new Person {Id = 0, Surname = "foo"});
		result.Errors.Count.ShouldEqual(0);
		shouldNotBeTrue.ShouldBeFalse();
	}

	[Fact]
	public void Invokes_custom_action_on_failure() {
		bool invoked = false;
		_validator.RuleFor(x => x.Surname).NotNull().OnAnyFailure(x => {
			invoked = true;
		});

		_validator.Validate(new Person());

		invoked.ShouldBeTrue();
	}

	[Fact]
	public async Task Invokes_custom_action_on_failure_async() {
		bool invoked = false;
		_validator.RuleFor(x => x.Surname).MustAsync((s, c) => Task.FromResult(s != null)).OnAnyFailure(x => {
			invoked = true;
		});

		await _validator.ValidateAsync(new Person());

		invoked.ShouldBeTrue();
	}

	[Fact]
	public void Passes_object_being_validated_to_action() {
		var person = new Person();
		Person validatedPerson = null;

		_validator.RuleFor(x => x.Surname).NotNull().OnAnyFailure(x => {
			validatedPerson = x;
		});

		_validator.Validate(person);

		person.ShouldBeTheSameAs(validatedPerson);
	}

	[Fact]
	public void Does_not_invoke_action_if_validation_success() {
		bool invoked = false;
		_validator.RuleFor(x => x.Surname).NotNull().OnAnyFailure(x => {
			invoked=true;
		});
		_validator.Validate(new Person() { Surname = "foo" });
		invoked.ShouldBeFalse();
	}

	[Fact]
	public void OnFailure_called_for_each_rule_using_foreach() {
		int invoked = 0;
		_validator.RuleForEach(person => person.Children).NotNull().OnFailure(person => {
			invoked += 1;
		});

		_validator.Validate(new Person {
			Children = new List<Person> {
				new Person(),
				null,
				null,
				new Person()
			}
		});

		invoked.ShouldEqual(2);
	}

	[Fact]
	public async Task OnFailure_called_for_each_rule_using_foreach_async() {
		int invoked = 0;
		_validator.RuleForEach(person => person.Children).MustAsync((x, cancel) => Task.FromResult(x != null)).OnFailure(person => {
			invoked += 1;
		});

		await _validator.ValidateAsync(new Person {
			Children = new List<Person> {
				new Person(),
				null,
				null,
				new Person()
			}
		});

		invoked.ShouldEqual(2);
	}
}

public class AddressValidatorWithOnFailure : AbstractValidator<Address> {
	public AddressValidatorWithOnFailure() {
		RuleFor(x => x.Postcode).NotNull().OnFailure((address, ctx, value) => {
			Debug.WriteLine(address.Line1);
		});
		RuleFor(x => x.Country).SetValidator(new ComplexValidationTester.CountryValidator());
	}
}
