namespace FluentValidation.Tests.Mvc6 {
    using System;
    using Attributes;
    using Internal;
    using Microsoft.AspNetCore.Mvc;
    using Mvc;
    using Results;

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

   /* public class SimplePropertyInterceptor : IValidatorInterceptor
    {
        readonly string[] properties = new[] { "Surname", "Forename" };

        public ValidationContext BeforeMvcValidation(ControllerContext cc, ValidationContext context)
        {
            var newContext = context.Clone(selector: new MemberNameValidatorSelector(properties));
            return newContext;
        }

        public ValidationResult AfterMvcValidation(ControllerContext cc, ValidationContext context, ValidationResult result)
        {
            return result;
        }
    }

    public class ClearErrorsInterceptor : IValidatorInterceptor
    {
        public ValidationContext BeforeMvcValidation(ControllerContext cc, ValidationContext context)
        {
            return null;
        }

        public ValidationResult AfterMvcValidation(ControllerContext cc, ValidationContext context, ValidationResult result)
        {
            return new ValidationResult();
        }
    }*/

   /* [Validator(typeof(PropertiesValidator2))]
    public class PropertiesTestModel2
    {
        public string Email { get; set; }
        public string Surname { get; set; }
        public string Forename { get; set; }
    }*/
/*
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
    }*/


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
            RuleFor(x => x.Id).NotNull().WithMessage("Foo");
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

}