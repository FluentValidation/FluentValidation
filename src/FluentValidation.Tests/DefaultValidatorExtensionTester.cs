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
// The latest version of this file can be found at https://github.com/jeremyskinner/FluentValidation
#endregion

namespace FluentValidation.Tests {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Internal;
	using Xunit;
	using Validators;

	
	public class DefaultValidatorExtensionTester {
		private AbstractValidator<Person> validator;

		public DefaultValidatorExtensionTester() {
			validator = new TestValidator();
		}

		[Fact]
		public void NotNull_should_create_NotNullValidator() {
			validator.RuleFor(x => x.Surname).NotNull();
			AssertValidator<NotNullValidator>();
		}

		[Fact]
		public void NotEmpty_should_create_NotEmptyValidator() {
			validator.RuleFor(x => x.Surname).NotEmpty();
			AssertValidator<NotEmptyValidator>();
		}

		[Fact]
		public void Empty_should_create_EmptyValidator() {
			validator.RuleFor(x => x.Surname).Empty();
			AssertValidator<EmptyValidator>();
		}

		[Fact]
		public void Length_should_create_LengthValidator() {
			validator.RuleFor(x => x.Surname).Length(1, 20);
			AssertValidator<LengthValidator>();
		}

		[Fact]
		public void Length_should_create_ExactLengthValidator() {
			validator.RuleFor(x => x.Surname).Length(5);
			AssertValidator<ExactLengthValidator>();
		}

		[Fact]
		public void Length_should_create_MaximumLengthValidator() {
			validator.RuleFor(x => x.Surname).MaximumLength(5);
			AssertValidator<MaximumLengthValidator>();
		}

		[Fact]
		public void Length_should_create_MinimumLengthValidator() {
			validator.RuleFor(x => x.Surname).MinimumLength(5);
			AssertValidator<MinimumLengthValidator>();
		}

		[Fact]
		public void NotEqual_should_create_NotEqualValidator_with_explicit_value() {
			validator.RuleFor(x => x.Surname).NotEqual("Foo");
			AssertValidator<NotEqualValidator>();
		}

		[Fact]
		public void NotEqual_should_create_NotEqualValidator_with_lambda() {
			validator.RuleFor(x => x.Surname).NotEqual(x => "Foo");
			AssertValidator<NotEqualValidator>();
		}

		[Fact]
		public void Equal_should_create_EqualValidator_with_explicit_value() {
			validator.RuleFor(x => x.Surname).Equal("Foo");
			AssertValidator<EqualValidator>();
		}

		[Fact]
		public void Equal_should_create_EqualValidator_with_lambda() {
			validator.RuleFor(x => x.Surname).Equal(x => "Foo");
			AssertValidator<EqualValidator>();
		}

		[Fact]
		public void Must_should_create_PredicteValidator() {
			validator.RuleFor(x => x.Surname).Must(x => true);
			AssertValidator<PredicateValidator>();
		}

		[Fact]
		public void Must_should_create_PredicateValidator_with_context() {
			validator.RuleFor(x => x.Surname).Must((x, val) => true);
			AssertValidator<PredicateValidator>();
		}

		[Fact]
		public void Must_should_create_PredicateValidator_with_PropertyValidatorContext() {
			var hasPropertyValidatorContext = false;
			this.validator.RuleFor(x => x.Surname).Must((x, val, ctx) => {
				hasPropertyValidatorContext = ctx != null;
				return true;
			});
			this.validator.Validate(new Person() {
				Surname = "Surname"
			});
			this.AssertValidator<PredicateValidator>();
			hasPropertyValidatorContext.ShouldBeTrue();
		}

		[Fact]
		public void MustAsync_should_create_AsyncPredicteValidator() {
			validator.RuleFor(x => x.Surname).MustAsync(async (x, cancel) => true);
			AssertValidator<AsyncPredicateValidator>();
		}

		[Fact]
		public void MustAsync_should_create_AsyncPredicateValidator_with_context() {
			validator.RuleFor(x => x.Surname).MustAsync(async (x, val) => true);
			AssertValidator<AsyncPredicateValidator>();
		}

		[Fact]
		public void MustAsync_should_create_AsyncPredicateValidator_with_PropertyValidatorContext() {
			var hasPropertyValidatorContext = false;
			this.validator.RuleFor(x => x.Surname).MustAsync(async (x, val, ctx, cancel) => {
				hasPropertyValidatorContext = ctx != null;
				return true;
			});
			this.validator.ValidateAsync(new Person {
				Surname = "Surname"
			}).Wait();
			this.AssertValidator<AsyncPredicateValidator>();
			hasPropertyValidatorContext.ShouldBeTrue();
		}

		[Fact]
		public void LessThan_should_create_LessThanValidator_with_explicit_value() {
			validator.RuleFor(x => x.Surname).LessThan("foo");
			AssertValidator<LessThanValidator>();
		}

		[Fact]
		public void LessThan_should_create_LessThanValidator_with_lambda() {
			validator.RuleFor(x => x.Surname).LessThan(x => "foo");
			AssertValidator<LessThanValidator>();
		}

		[Fact]
		public void LessThanOrEqual_should_create_LessThanOrEqualValidator_with_explicit_value() {
			validator.RuleFor(x => x.Surname).LessThanOrEqualTo("foo");
			AssertValidator<LessThanOrEqualValidator>();
		}

		[Fact]
		public void LessThanOrEqual_should_create_LessThanOrEqualValidator_with_lambda() {
			validator.RuleFor(x => x.Surname).LessThanOrEqualTo(x => "foo");
			AssertValidator<LessThanOrEqualValidator>();
		}

		[Fact]
		public void LessThanOrEqual_should_create_LessThanOrEqualValidator_with_lambda_with_other_Nullable() {
			validator.RuleFor(x => x.NullableInt).LessThanOrEqualTo(x => x.OtherNullableInt);
			AssertValidator<LessThanOrEqualValidator>();
		}

		[Fact]
		public void GreaterThan_should_create_GreaterThanValidator_with_explicit_value() {
			validator.RuleFor(x => x.Surname).GreaterThan("foo");
			AssertValidator<GreaterThanValidator>();
		}

		[Fact]
		public void GreaterThan_should_create_GreaterThanValidator_with_lambda() {
			validator.RuleFor(x => x.Surname).GreaterThan(x => "foo");
			AssertValidator<GreaterThanValidator>();
		}

		[Fact]
		public void GreaterThanOrEqual_should_create_GreaterThanOrEqualValidator_with_explicit_value() {
			validator.RuleFor(x => x.Surname).GreaterThanOrEqualTo("foo");
			AssertValidator<GreaterThanOrEqualValidator>();
		}

		[Fact]
		public void GreaterThanOrEqual_should_create_GreaterThanOrEqualValidator_with_lambda() {
			validator.RuleFor(x => x.Surname).GreaterThanOrEqualTo(x => "foo");
			AssertValidator<GreaterThanOrEqualValidator>();
		}

		[Fact]
		public void GreaterThanOrEqual_should_create_GreaterThanOrEqualValidator_with_lambda_with_other_Nullable() {
			validator.RuleFor(x => x.NullableInt).GreaterThanOrEqualTo(x => x.OtherNullableInt);
			AssertValidator<GreaterThanOrEqualValidator>();
		}
		[Fact]
		public void MustAsync_should_not_throw_InvalidCastException() {
			var model = new Model
			{
				Ids = new Guid[0]
			};
			var validator = new AsyncModelTestValidator();
			// this fails with "Specified cast is not valid" error
			var result = validator.ValidateAsync(model).Result;
			result.IsValid.ShouldBeTrue();
		}
		private void AssertValidator<TValidator>() {
			var rule = (PropertyRule)validator.First();
			rule.CurrentValidator.ShouldBe<TValidator>();
		}

		class Model
		{
			public IEnumerable<Guid> Ids { get; set; }
		}

		class AsyncModelTestValidator : AbstractValidator<Model>
		{
			public AsyncModelTestValidator()
			{
				RuleForEach(m => m.Ids)
					.MustAsync((g, cancel) => Task.FromResult(true));
			}
		}
	}

}