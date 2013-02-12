#region License
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
// The latest version of this file can be found at http://www.codeplex.com/FluentValidation
#endregion

namespace FluentValidation.Tests {
	using System.Linq;
	using Internal;
	using NUnit.Framework;
	using Validators;

	[TestFixture]
	public class DefaultValidatorExtensionTester {
		private AbstractValidator<Person> validator;

		[SetUp]
		public void Setup() {
			validator = new TestValidator();
		}

		[Test]
		public void NotNull_should_create_NotNullValidator() {
			validator.RuleFor(x => x.Surname).NotNull();
			AssertValidator<NotNullValidator>();
		}

		[Test]
		public void NotEmpty_should_create_NotEmptyValidator() {
			validator.RuleFor(x => x.Surname).NotEmpty();
			AssertValidator<NotEmptyValidator>();
		}

		[Test]
		public void Length_should_create_LengthValidator() {
			validator.RuleFor(x => x.Surname).Length(1, 20);
			AssertValidator<LengthValidator>();
		}

		[Test]
		public void Length_should_create_ExactLengthValidator() {
			validator.RuleFor(x => x.Surname).Length(5);
			AssertValidator<ExactLengthValidator>();
		}

		[Test]
		public void NotEqual_should_create_NotEqualValidator_with_explicit_value() {
			validator.RuleFor(x => x.Surname).NotEqual("Foo");
			AssertValidator<NotEqualValidator>();
		}

		[Test]
		public void NotEqual_should_create_NotEqualValidator_with_lambda() {
			validator.RuleFor(x => x.Surname).NotEqual(x => "Foo");
			AssertValidator<NotEqualValidator>();
		}

		[Test]
		public void Equal_should_create_EqualValidator_with_explicit_value() {
			validator.RuleFor(x => x.Surname).Equal("Foo");
			AssertValidator<EqualValidator>();
		}

		[Test]
		public void Equal_should_create_EqualValidator_with_lambda() {
			validator.RuleFor(x => x.Surname).Equal(x => "Foo");
			AssertValidator<EqualValidator>();
		}

		[Test]
		public void Must_should_create_PredicteValidator() {
			validator.RuleFor(x => x.Surname).Must(x => true);
			AssertValidator<PredicateValidator>();
		}

		[Test]
		public void Must_should_create_PredicateValidator_with_context() {
			validator.RuleFor(x => x.Surname).Must((x, val) => true);
			AssertValidator<PredicateValidator>();
		}

        [Test]
        public void Must_should_create_PredicateValidator_with_PropertyValidatorContext() {
            var hasPropertyValidatorContext = false;
            validator.RuleFor(x => x.Surname).Must((x, val, ctx) => {
                hasPropertyValidatorContext = ctx != null;
                return true; });
            validator.Validate(new Person() {Surname = "Surname"});
            AssertValidator<PredicateValidator>();
            hasPropertyValidatorContext.ShouldBeTrue();
        }

		[Test]
		public void LessThan_should_create_LessThanValidator_with_explicit_value() {
			validator.RuleFor(x => x.Surname).LessThan("foo");
			AssertValidator<LessThanValidator>();
		}

		[Test]
		public void LessThan_should_create_LessThanValidator_with_lambda() {
			validator.RuleFor(x => x.Surname).LessThan(x => "foo");
			AssertValidator<LessThanValidator>();
		}

		[Test]
		public void LessThanOrEqual_should_create_LessThanOrEqualValidator_with_explicit_value() {
			validator.RuleFor(x => x.Surname).LessThanOrEqualTo("foo");
			AssertValidator<LessThanOrEqualValidator>();
		}

		[Test]
		public void LessThanOrEqual_should_create_LessThanOrEqualValidator_with_lambda() {
			validator.RuleFor(x => x.Surname).LessThanOrEqualTo(x => "foo");
			AssertValidator<LessThanOrEqualValidator>();
		}

    [Test]
    public void LessThanOrEqual_should_create_LessThanOrEqualValidator_with_lambda_with_other_Nullable() {
      validator.RuleFor(x => x.NullableInt).LessThanOrEqualTo(x => x.OtherNullableInt);
      AssertValidator<LessThanOrEqualValidator>();
    }

		[Test]
		public void GreaterThan_should_create_GreaterThanValidator_with_explicit_value() {
			validator.RuleFor(x => x.Surname).GreaterThan("foo");
			AssertValidator<GreaterThanValidator>();
		}

		[Test]
		public void GreaterThan_should_create_GreaterThanValidator_with_lambda() {
			validator.RuleFor(x => x.Surname).GreaterThan(x => "foo");
			AssertValidator<GreaterThanValidator>();
		}

		[Test]
		public void GreaterThanOrEqual_should_create_GreaterThanOrEqualValidator_with_explicit_value() {
			validator.RuleFor(x => x.Surname).GreaterThanOrEqualTo("foo");
			AssertValidator<GreaterThanOrEqualValidator>();
		}

		[Test]
		public void GreaterThanOrEqual_should_create_GreaterThanOrEqualValidator_with_lambda() {
			validator.RuleFor(x => x.Surname).GreaterThanOrEqualTo(x => "foo");
			AssertValidator<GreaterThanOrEqualValidator>();
		}

    [Test]
    public void GreaterThanOrEqual_should_create_GreaterThanOrEqualValidator_with_lambda_with_other_Nullable() {
      validator.RuleFor(x => x.NullableInt).GreaterThanOrEqualTo(x => x.OtherNullableInt);
      AssertValidator<GreaterThanOrEqualValidator>();
    }

		private void AssertValidator<TValidator>() {
			var rule = (PropertyRule)validator.First();
			rule.CurrentValidator.ShouldBe<TValidator>();
		}
	}
}