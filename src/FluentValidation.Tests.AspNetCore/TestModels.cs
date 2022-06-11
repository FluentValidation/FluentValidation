namespace FluentValidation.Tests;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using ValidationResult = Results.ValidationResult;

public class TestModel5 {
	public int Id { get; set; }
	public bool SomeBool { get; set; }
}

public class TestModel5Validator : AbstractValidator<TestModel5> {
	public TestModel5Validator() {
		//force a complex rule
		RuleFor(x => x.SomeBool).Must(x => x == true);
		RuleFor(x => x.Id).NotEmpty();
	}
}

public class SimplePropertyInterceptor : FluentValidation.AspNetCore.IValidatorInterceptor {
	readonly string[] properties = new[] {"Surname", "Forename"};

	public IValidationContext BeforeAspNetValidation(ActionContext cc, IValidationContext context) {
		var newContext = new ValidationContext<object>(context.InstanceToValidate, context.PropertyChain, new FluentValidation.Internal.MemberNameValidatorSelector(properties));
		return newContext;
	}

	public ValidationResult AfterAspNetValidation(ActionContext cc, IValidationContext context, ValidationResult result) {
		return result;
	}
}

public class ClearErrorsInterceptor : FluentValidation.AspNetCore.IValidatorInterceptor {
	public IValidationContext BeforeAspNetValidation(ActionContext cc, IValidationContext context) {
		return null;
	}

	public ValidationResult AfterAspNetValidation(ActionContext cc, IValidationContext context, ValidationResult result) {
		return new ValidationResult();
	}
}

public class PropertiesTestModel2 {
	public string Email { get; set; }
	public string Surname { get; set; }
	public string Forename { get; set; }
}

public class PropertiesValidator2 : AbstractValidator<PropertiesTestModel2>, IValidatorInterceptor {
	public PropertiesValidator2() {
		RuleFor(x => x.Email).NotEqual("foo");
		RuleFor(x => x.Surname).NotEqual("foo");
		RuleFor(x => x.Forename).NotEqual("foo");
	}

	public IValidationContext BeforeAspNetValidation(ActionContext controllerContext, IValidationContext commonContext) {
		return commonContext;
	}

	public ValidationResult AfterAspNetValidation(ActionContext controllerContext, IValidationContext commonContext, ValidationResult result) {
		return new ValidationResult(); //empty errors
	}
}


public class PropertiesTestModel {
	public string Email { get; set; }
	public string Surname { get; set; }
	public string Forename { get; set; }
}


public class PropertiesValidator : AbstractValidator<PropertiesTestModel> {
	public PropertiesValidator() {
		RuleFor(x => x.Email).NotEqual("foo");
		RuleFor(x => x.Surname).NotEqual("foo");
		RuleFor(x => x.Forename).NotEqual("foo");
	}
}

public class RulesetTestModel {
	public string Email { get; set; }
	public string Surname { get; set; }
	public string Forename { get; set; }
}

public class RulesetTestValidator : AbstractValidator<RulesetTestModel> {
	public RulesetTestValidator() {
		RuleFor(x => x.Email).NotEqual("foo");

		RuleSet("Names", () => {
			RuleFor(x => x.Surname).NotEqual("foo");
			RuleFor(x => x.Forename).NotEqual("foo");
		});
	}
}

public class TestModelWithOverridenMessageValueType {
	public int Id { get; set; }
}

public class TestModelWithOverridenPropertyNameValueType {
	public int Id { get; set; }
}

public class TestModelWithOverridenMessageValueTypeValidator : AbstractValidator<TestModelWithOverridenMessageValueType> {
	public TestModelWithOverridenMessageValueTypeValidator() {
		RuleFor(x => x.Id).NotEmpty().WithMessage("Foo");
	}
}

public class TestModelWithOverridenPropertyNameValidator : AbstractValidator<TestModelWithOverridenPropertyNameValueType> {
	public TestModelWithOverridenPropertyNameValidator() {
		RuleFor(x => x.Id).NotEmpty().WithName("Foo");
	}
}


public class TestModel2 {
}

public class TestModel {
	public string Name { get; set; }
}

public class TestModelValidator : AbstractValidator<TestModel> {
	public TestModelValidator() {
		RuleFor(x => x.Name).NotNull().WithMessage("Validation Failed");
	}
}

public class TestModel3 {
	public int Id { get; set; }
}

public class TestModelValidator3 : AbstractValidator<TestModel3> {
	public TestModelValidator3() {
		RuleFor(x => x.Id).NotEmpty().WithMessage("Validation failed");
	}
}

public class TestModelWithoutValidator {
	public int Id { get; set; }
}

public class TestModel4 {
	public string Surname { get; set; }
	public string Forename { get; set; }
	public string Email { get; set; }
	public DateTime DateOfBirth { get; set; }
	public string Address1 { get; set; }
}


public class TestModel4Validator : AbstractValidator<TestModel4> {
	public TestModel4Validator() {
		RuleFor(x => x.Surname).NotEqual(x => x.Forename);

		RuleFor(x => x.Email)
			.EmailAddress();

		RuleFor(x => x.Address1).NotEmpty();
	}
}

public class TestModel6 {
	public int Id { get; set; }
}

public class TestModel6Validator : AbstractValidator<TestModel6> {
	public TestModel6Validator() {
		//This ctor is intentionally blank.
	}
}

// No attribute- only resolved using service provider.
public class LifecycleTestModel {
}

public class LifecycleTestValidator : AbstractValidator<LifecycleTestModel> {
	public LifecycleTestValidator() {
		RuleFor(x => x).Custom((x, ctx) => { ctx.AddFailure(new ValidationFailure("Foo", GetHashCode().ToString())); });
	}
}

public class CollectionTestModel {
	public string Name { get; set; }
}

public class CollectionTestModelValidator : AbstractValidator<CollectionTestModel> {
	public CollectionTestModelValidator() {
		RuleFor(x => x.Name).NotEqual("foo");
	}
}

public class MultipleErrorsModel {
	public string Name { get; set; }
}

public class MultipleErrorsModelValidator : AbstractValidator<MultipleErrorsModel> {
	public MultipleErrorsModelValidator() {
		RuleFor(x => x.Name).Equal("foo").Equal("bar");
	}
}

public class ModelThatImplementsIEnumerableValidator : AbstractValidator<ModelThatImplementsIEnumerable> {
	public ModelThatImplementsIEnumerableValidator() {
		RuleFor(x => x.Name).NotNull().WithMessage("Foo");
	}
}

public class ModelThatImplementsIEnumerable : IEnumerable<ModelPartOfIenumerable> {
	public string Name { get; set; }

	private List<ModelPartOfIenumerable> _something = new List<ModelPartOfIenumerable> {
		new ModelPartOfIenumerable(),
		new ModelPartOfIenumerable()
	};

	IEnumerator IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}

	public IEnumerator<ModelPartOfIenumerable> GetEnumerator() {
		return _something.GetEnumerator();
	}
}

public class PartofEnumerableValidator : AbstractValidator<ModelPartOfIenumerable> {
	public PartofEnumerableValidator() {
		RuleFor(x => x.Name).NotNull().WithMessage("Bar");
	}
}

public class ModelPartOfIenumerable {
	public string Name { get; set; }
}

public class MultiValidationModel {
	[Required]
	public string Name { get; set; }


	public string SomeOtherProperty { get; set; }
}

public class MultiValidationValidator : AbstractValidator<MultiValidationModel> {
	public MultiValidationValidator() {
		RuleFor(x => x.SomeOtherProperty).NotNull();
	}
}

public class MultiValidationModel2 {
	[Required]
	public string Name { get; set; }
}

public class MultiValidationValidator2 : AbstractValidator<MultiValidationModel2> {
	public MultiValidationValidator2() {
		RuleFor(x => x.Name).Must(x => x == "foo");
	}
}


public class MultiValidationModel3 {
	public string Name { get; set; }
	public ChildModel5 Child { get; set; } = new ChildModel5();
}

public class MultiValidationValidator3 : AbstractValidator<MultiValidationModel3> {
	public MultiValidationValidator3() {
		RuleFor(x => x.Name).NotNull();
		RuleFor(x => x.Child).SetValidator(new InlineValidator<ChildModel5>() {
			v => v.RuleFor(x => x.Name2).NotNull()
		});
	}
}

public class ChildModel5 {
	[Required]
	public string Name { get; set; }

	public string Name2 { get; set; }
}

public class DataAnnotationsModel {
	[Required]
	public string Name { get; set; }
}

public class ParentModel {
	public ChildModel Child { get; set; } = new ChildModel();
}

public class ChildModel {
	public string Name { get; set; }
}

public class ParentModelValidator : AbstractValidator<ParentModel> {
}

public class ChildModelValidator : AbstractValidator<ChildModel> {
	public ChildModelValidator() {
		RuleFor(x => x.Name).NotNull();
	}
}

public class ImplementsIValidatableObjectModel : IValidatableObject {
	public string Name { get; set; }
	public string Name2 { get; set; }

	public IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext) {
		yield return new System.ComponentModel.DataAnnotations.ValidationResult("Fail", new[] {"Name2"});
	}
}


public class ImplementsIValidatableObjectValidator : AbstractValidator<ImplementsIValidatableObjectModel> {
	public ImplementsIValidatableObjectValidator() {
		RuleFor(x => x.Name).NotNull();
	}
}

public class ParentModel2 {
	public ChildModel2 Child { get; set; } = new ChildModel2();
	public string Name { get; set; }
}

public class ChildModel2 : IValidatableObject {
	public string Name { get; set; }
	public string Name2 { get; set; }

	public IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext) {
		yield return new System.ComponentModel.DataAnnotations.ValidationResult("Fail", new[] {"Name2"});
	}
}


public class ParentModel2Validator : AbstractValidator<ParentModel2> {
	public ParentModel2Validator() {
		RuleFor(x => x.Name).NotNull();
	}
}

public class ChildModel2Validator : AbstractValidator<ChildModel2> {
	public ChildModel2Validator() {
		RuleFor(x => x.Name).NotNull();
	}
}


public class ParentModel3 {
	public ChildModel3 Child { get; set; } = new ChildModel3();
}

public class ChildModel3 {
	public string Name { get; set; }

	[Required]
	public string Name2 { get; set; }
}

public class ParentModelValidator3 : AbstractValidator<ParentModel3> {
}

public class ChildModelValidator3 : AbstractValidator<ChildModel3> {
	public ChildModelValidator3() {
		RuleFor(x => x.Name).NotNull();
	}
}

public class ParentModel4 {
	public ChildModel4 Child { get; set; } = new ChildModel4();
}

public class ChildModel4 {
	public string Name { get; set; }
}

public class ParentModel4Validator : AbstractValidator<ParentModel4> {
	public ParentModel4Validator() {
		RuleFor(x => x.Child).SetValidator(new ChildModel4Validator());
	}
}

public class ChildModel4Validator : AbstractValidator<ChildModel4> {
	public ChildModel4Validator() {
		RuleFor(x => x.Name).NotNull();
	}
}

public class ParentModel5 {
	public ChildModel Child { get; set; }
}

public class ParentModel5Validator : AbstractValidator<ParentModel5> {
}


public class ParentModel6 {
	public List<ChildModel> Children { get; set; } = new List<ChildModel>();
}

public class ParentModel6Validator : AbstractValidator<ParentModel6> {
	public ParentModel6Validator() {
	}
}

public class ParentModel7 {
	public List<ChildModel7> Children { get; set; } = new List<ChildModel7>();
}

public class ChildModel7 {
	[Required]
	public string Name { get; set; }
}

public class InjectsExplicitChildValidator : AbstractValidator<ParentModel> {
	public InjectsExplicitChildValidator() {
		RuleFor(x => x.Child).InjectValidator();
	}
}

public class InjectedChildValidator : AbstractValidator<ChildModel> {
	public InjectedChildValidator() {
		RuleFor(x => x.Name).NotNull().WithMessage("NotNullInjected");
	}
}

public class InjectsExplicitChildValidatorCollection : AbstractValidator<ParentModel6> {
	public InjectsExplicitChildValidatorCollection() {
		RuleForEach(x => x.Children).InjectValidator();
	}
}

public class BadAsyncModel {
	public int Id { get; set; }
}

// Not allowed with ASP.NET auto-validation.
// Async calls during auto validation should trigger an exception.
public class BadAsyncValidator : AbstractValidator<BadAsyncModel> {
	public BadAsyncValidator() {
		RuleFor(x => x.Id).MustAsync(async (id, cancel) => {
			var client = new HttpClient();
			var resp = await client.GetAsync("https://dotnet.microsoft.com/");
			return resp.IsSuccessStatusCode;
		});
	}
}
