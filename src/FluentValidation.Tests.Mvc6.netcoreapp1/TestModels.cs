namespace FluentValidation.Tests.AspNetCore {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using FluentValidation;
	using FluentValidation.Attributes;
	using FluentValidation.AspNetCore;
	using FluentValidation.Results;
	using Microsoft.AspNetCore.Mvc;
	using ValidationContext = FluentValidation.ValidationContext;
	using ValidationResult = Results.ValidationResult;

	[Validator(typeof(TestModel5Validator))]
    public class TestModel5
    {
        public int Id { get; set; }
        public bool SomeBool { get; set; }
    }

    public class TestModel5Validator : AbstractValidator<TestModel5>
    {
        public TestModel5Validator()
        {
            //force a complex rule
            RuleFor(x => x.SomeBool).Must(x => x == true);
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public class SimplePropertyInterceptor : FluentValidation.AspNetCore.IValidatorInterceptor
    {
        readonly string[] properties = new[] { "Surname", "Forename" };

        public ValidationContext BeforeMvcValidation(ControllerContext cc, ValidationContext context)
        {
            var newContext = context.Clone(selector: new FluentValidation.Internal.MemberNameValidatorSelector(properties));
            return newContext;
        }

        public ValidationResult AfterMvcValidation(ControllerContext cc, ValidationContext context, ValidationResult result)
        {
            return result;
        }
    }

    public class ClearErrorsInterceptor : FluentValidation.AspNetCore.IValidatorInterceptor
    {
        public ValidationContext BeforeMvcValidation(ControllerContext cc, ValidationContext context)
        {
            return null;
        }

        public ValidationResult AfterMvcValidation(ControllerContext cc, ValidationContext context, ValidationResult result)
        {
            return new ValidationResult();
        }
    }

    [Validator(typeof(PropertiesValidator2))]
    public class PropertiesTestModel2
    {
        public string Email { get; set; }
        public string Surname { get; set; }
        public string Forename { get; set; }
    }
    public class PropertiesValidator2 : AbstractValidator<PropertiesTestModel2>, IValidatorInterceptor
    {
        public PropertiesValidator2()
        {
            RuleFor(x => x.Email).NotEqual("foo");
            RuleFor(x => x.Surname).NotEqual("foo");
            RuleFor(x => x.Forename).NotEqual("foo");
        }

        public ValidationContext BeforeMvcValidation(ControllerContext controllerContext, ValidationContext validationContext)
        {
            return validationContext;
        }

        public ValidationResult AfterMvcValidation(ControllerContext controllerContext, ValidationContext validationContext, ValidationResult result)
        {
            return new ValidationResult(); //empty errors
        }
    }


    [Validator(typeof(PropertiesValidator))]
    public class PropertiesTestModel
    {
        public string Email { get; set; }
        public string Surname { get; set; }
        public string Forename { get; set; }
    }


    public class PropertiesValidator : AbstractValidator<PropertiesTestModel>
    {
        public PropertiesValidator()
        {
            RuleFor(x => x.Email).NotEqual("foo");
            RuleFor(x => x.Surname).NotEqual("foo");
            RuleFor(x => x.Forename).NotEqual("foo");
        }
    }

    [Validator(typeof(RulesetTestValidator))]
    public class RulesetTestModel
    {
        public string Email { get; set; }
        public string Surname { get; set; }
        public string Forename { get; set; }
    }

    public class RulesetTestValidator : AbstractValidator<RulesetTestModel>
    {
        public RulesetTestValidator()
        {
            RuleFor(x => x.Email).NotEqual("foo");

            RuleSet("Names", () => {
                RuleFor(x => x.Surname).NotEqual("foo");
                RuleFor(x => x.Forename).NotEqual("foo");
            });
        }
    }

    [Validator(typeof(TestModelWithOverridenMessageValueTypeValidator))]
    public  class TestModelWithOverridenMessageValueType
    {
        public int Id { get; set; }
    }

    [Validator(typeof(TestModelWithOverridenPropertyNameValidator))]
    public class TestModelWithOverridenPropertyNameValueType
    {
        public int Id { get; set; }
    }

    public class TestModelWithOverridenMessageValueTypeValidator : AbstractValidator<TestModelWithOverridenMessageValueType>
    {
        public TestModelWithOverridenMessageValueTypeValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Foo");
        }
    }

    public class TestModelWithOverridenPropertyNameValidator : AbstractValidator<TestModelWithOverridenPropertyNameValueType>
    {
        public TestModelWithOverridenPropertyNameValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithName("Foo");

        }
    }




    public class TestModel2
    {
    }

    [Validator(typeof(TestModelValidator))]
    public class TestModel
    {
        public string Name { get; set; }
    }

    public class TestModelValidator : AbstractValidator<TestModel>
    {
        public TestModelValidator()
        {
            RuleFor(x => x.Name).NotNull().WithMessage("Validation Failed");
        }
    }

    [Validator(typeof(TestModelValidator3))]
    public class TestModel3
    {
        public int Id { get; set; }
    }

    public class TestModelValidator3 : AbstractValidator<TestModel3>
    {
        public TestModelValidator3()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Validation failed");
        }
    }

    public class TestModelWithoutValidator
    {
        public int Id { get; set; }
    }

    [Validator(typeof(TestModel4Validator))]
    public class TestModel4
    {
        public string Surname { get; set; }
        public string Forename { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address1 { get; set; }
    }



    public class TestModel4Validator : AbstractValidator<TestModel4>
    {
        public TestModel4Validator()
        {
            RuleFor(x => x.Surname).NotEqual(x => x.Forename);

            RuleFor(x => x.Email)
                .EmailAddress();

            RuleFor(x => x.Address1).NotEmpty();
        }
    }

    [Validator(typeof(TestModel6Validator))]
    public class TestModel6
    {
        public int Id { get; set; }
    }

    public class TestModel6Validator : AbstractValidator<TestModel6>
    {
        public TestModel6Validator()
        {
            //This ctor is intentionally blank.
        }
    }

	// No attribute- only resolved using service provider. 
	public class LifecycleTestModel {  }

	public class LifecycleTestValidator : AbstractValidator<LifecycleTestModel> {
		public LifecycleTestValidator() {
			Custom(x => {
				return new ValidationFailure("Foo", GetHashCode().ToString());
			});
		}
	}

	[Validator(typeof(CollectionTestModelValidator))]
	public class CollectionTestModel {
		public string Name { get; set; }
	}

	public class CollectionTestModelValidator : AbstractValidator<CollectionTestModel> {
		public CollectionTestModelValidator() {
			RuleFor(x => x.Name).NotEqual("foo");
		}
	}

	[Validator(typeof(MultipleErrorsModelValidator))]
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

	[Validator(typeof(MultiValidationValidator))]
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

	[Validator(typeof(MultiValidationValidator2))]
	public class MultiValidationModel2 {
		[Required]
		public string Name { get; set; }

	}

	public class MultiValidationValidator2 : AbstractValidator<MultiValidationModel2> {
		public MultiValidationValidator2() {
			RuleFor(x => x.Name).Must(x => x == "foo");
		}
	}


	[Validator(typeof(MultiValidationValidator3))]
	public class MultiValidationModel3 {
		public string Name { get; set; }
		public ChildModel5 Child {get; set;} = new ChildModel5();
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

	[Validator(typeof(ParentModelValidator))]
	public class ParentModel {
		public ChildModel Child { get; set; } = new ChildModel();
	}

	[Validator(typeof(ChildModelValidator))]
	public class ChildModel {
		public string Name { get; set; }
	}

	public class ParentModelValidator : AbstractValidator<ParentModel> { }

	public class ChildModelValidator : AbstractValidator<ChildModel> {
		public ChildModelValidator() {
			RuleFor(x => x.Name).NotNull();
		}
	}

	[Validator(typeof(ImplementsIValidatableObjectValidator))]
	public class ImplementsIValidatableObjectModel : IValidatableObject {
		public string Name { get; set; }
		public string Name2 { get; set; }
		public IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext) {
			yield return new System.ComponentModel.DataAnnotations.ValidationResult("Fail", new[] { "Name2"});
		}
	}


	public class ImplementsIValidatableObjectValidator : AbstractValidator<ImplementsIValidatableObjectModel> {
		public ImplementsIValidatableObjectValidator() {
			RuleFor(x => x.Name).NotNull();
		}
	}

	[Validator(typeof(ParentModel2Validator))]
	public class ParentModel2 {
		public ChildModel2 Child { get; set; } = new ChildModel2();
		public string Name { get; set; }
	}

	[Validator(typeof(ChildModel2Validator))]
	public class ChildModel2 : IValidatableObject {
		public string Name { get; set; }
		public string Name2 { get; set; }

		public IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext) {
			yield return new System.ComponentModel.DataAnnotations.ValidationResult("Fail", new[] { "Name2"});
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



	[Validator(typeof(ParentModelValidator3))]
	public class ParentModel3 {
		public ChildModel3 Child { get; set; } = new ChildModel3();
	}

	[Validator(typeof(ChildModelValidator3))]
	public class ChildModel3 {
		public string Name { get; set; }

		[Required]
		public string Name2 { get; set; }
	}

	public class ParentModelValidator3 : AbstractValidator<ParentModel3> { }

	public class ChildModelValidator3 : AbstractValidator<ChildModel3> {
		public ChildModelValidator3() {
			RuleFor(x => x.Name).NotNull();
		}
	}

	[Validator(typeof(ParentModel4Validator))]
	public class ParentModel4 {
		public ChildModel4 Child { get; set; } = new ChildModel4();
	}

	[Validator(typeof(ChildModel4Validator))]
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

	[Validator(typeof(ParentModel5Validator))]
	public class ParentModel5 {
		public ChildModel Child { get; set; }
	}

	public class ParentModel5Validator : AbstractValidator<ParentModel5> { }

}