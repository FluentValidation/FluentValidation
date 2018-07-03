﻿#region License
// Copyright (c) Jeremy Skinner (http://www.jeremyskinner.co.uk)
// 
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
// 
// http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
// 
// The latest version of this file can be found at https://github.com/JeremySkinner/FluentValidation
#endregion
namespace FluentValidation.Tests.WebApi {
	using System;
	using System.Collections.Generic;
	using System.Web.Http.Controllers;
	using Attributes;
	using FluentValidation.WebApi;
	using Results;

	[Validator(typeof(TestModel5Validator))]
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

	public class TestModel2 {
	}

	[Validator(typeof(TestModelValidator))]
	public class TestModel {
		public string Name { get; set; }
	}

	public class TestModelValidator : AbstractValidator<TestModel> {
		public TestModelValidator() {
			RuleFor(x => x.Name).NotNull().WithMessage("Validation Failed");
		}
	}

	[Validator(typeof(TestModelValidator3))]
	public class TestModel3 {
		public int Id { get; set; }
	}

	public class TestModelValidator3 : AbstractValidator<TestModel3> {
		public TestModelValidator3() {
			RuleFor(x => x.Id).NotNull().WithMessage("Validation failed");
		}
	}

	public class TestModelWithoutValidator {
		public int Id { get; set; }
	}

	[Validator(typeof(TestModel4Validator))]
	public class TestModel4 {
		public string Surname { get; set; }
		public string Forename { get; set; }
		public string Email { get; set; }
		public DateTime DateOfBirth { get; set; }
		public string Address1 { get; set; }
	}

	public class TestModel4Validator : AbstractValidator<TestModel4> {
		public TestModel4Validator() {
			RuleFor(x => x.Surname).NotEqual(x => x.Forename).WithMessage("Should not be equal: {PropertyValue} {ComparisonValue}");

			RuleFor(x => x.Email)
				.EmailAddress();

			RuleFor(x => x.Address1).NotEmpty();
		}
	}

	[Validator(typeof(TestModel6Validator))]
	public class TestModel6 {
		public int Id { get; set; }
	}

	public class TestModel6Validator : AbstractValidator<TestModel6> {
		public TestModel6Validator() {
			//This ctor is intentionally blank.
		}
	}

	[Validator(typeof(TestModel7Validator))]
	public class TestModel7 {
		public int AnIntProperty { get; set; }
		public int CustomProperty { get; set; }
	}

	public class TestModel7Validator : AbstractValidator<TestModel7> {
		public TestModel7Validator() {
			RuleFor(x => x.AnIntProperty).LessThan(10).WithMessage("Less than 10");
			RuleFor(x=>x).Custom((model,ctx) => {
					if (model.CustomProperty == 14) {
						ctx.AddFailure(new ValidationFailure("CustomProperty", "Cannot be 14"));
					}
				});
		}
	}

	public class TestModel8Validator : AbstractValidator<TestModel8> {
		public TestModel8Validator() {
			RuleFor(m => m.Name).NotEmpty().WithMessage("Validation failed");
		}
	}

	[Validator(typeof(TestModel8Validator))]
	public class TestModel8 {
		public string Name { get; set; }
        public int Age { get; set; }

	}

    [Validator(typeof(TestModel9Validator))]
    public class TestModel9 {
        public string Name { get; set; }
        public List<TestModel> Children { get; set; }
    }

    public class TestModel9Validator : AbstractValidator<TestModel9> {
        public TestModel9Validator() {
            RuleFor(m => m.Name).NotEmpty();
        }
    }

    [Validator(typeof(TestModel10Validator))]
    public class TestModel10
    {
        public string Name { get; set; }
        public TestModel Child { get; set; }
    }

    public class TestModel10Validator : AbstractValidator<TestModel10> {
        public TestModel10Validator() {
            RuleFor(m => m.Name).NotEmpty();
        }
    }
	
	[Validator(typeof(TestModel11Validator))]
	public class TestModel11 {
		public string Name { get; set; }
		public TestModel Child { get; set; }
	}

	public class TestModel11Validator : AbstractValidator<TestModel11> {
		public TestModel11Validator() {
			RuleFor(m => m.Name).NotEmpty().WithMessage("Validation failed");
		}
	}
	
	[Validator(typeof(PropertiesValidator))]
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

	[Validator(typeof(RulesetTestValidator))]
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
	
	[Validator(typeof(PropertiesValidator2))]
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

		public ValidationContext BeforeMvcValidation(HttpActionContext controllerContext, ValidationContext validationContext) {
			return validationContext;
		}

		public ValidationResult AfterMvcValidation(HttpActionContext controllerContext, ValidationContext validationContext, ValidationResult result) {
			return new ValidationResult(); //empty errors
		}
	}

	public class SimplePropertyInterceptor : IValidatorInterceptor {
		readonly string[] properties = new[] { "Surname", "Forename" };

		public ValidationContext BeforeMvcValidation(HttpActionContext cc, ValidationContext context) {
			var newContext = context.Clone(selector: new FluentValidation.Internal.MemberNameValidatorSelector(properties));
			return newContext;
		}

		public ValidationResult AfterMvcValidation(HttpActionContext cc, ValidationContext context, ValidationResult result) {
			return result;
		}
	}

	public class ClearErrorsInterceptor : IValidatorInterceptor {
		public ValidationContext BeforeMvcValidation(HttpActionContext cc, ValidationContext context) {
			return null;
		}

		public ValidationResult AfterMvcValidation(HttpActionContext cc, ValidationContext context, ValidationResult result) {
			return new ValidationResult();
		}
	}
}