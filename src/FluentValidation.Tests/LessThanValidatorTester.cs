#region License
// Copyright 2008-2009 Jeremy Skinner (http://www.jeremyskinner.co.uk)
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
	using System;
	using System.Globalization;
	using System.Linq.Expressions;
	using System.Threading;
	using NUnit.Framework;
	using Validators;

	[TestFixture]
	public class LessThanValidatorTester {
		LessThanValidator<Person, int> validator;
		int value = 1;

		[SetUp]
		public void Setup() {
			validator = new LessThanValidator<Person, int>(x => value);
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
		}

		[Test]
		public void Should_fail_when_greater_than_input() {
			var result = validator.Validate(new PropertyValidatorContext<Person, int>(null, null, x => 2));
			result.IsValid.ShouldBeFalse();
		}

		[Test]
		public void Should_succeed_when_less_than_input() {
			var result = validator.Validate(new PropertyValidatorContext<Person, int>(null, null, x => 0));
			result.IsValid.ShouldBeTrue();
		}

		[Test]
		public void Should_fail_when_equal_to_input() {
			var result = validator.Validate(new PropertyValidatorContext<Person, int>(null, null, x => value));
			result.IsValid.ShouldBeFalse();
		}

		[Test]
		public void Should_set_default_validation_message_when_validation_fails() {
			var result = validator.Validate(new PropertyValidatorContext<Person, int>("Discount", new Person(), x => 2));
			result.Error.ShouldEqual("'Discount' must be less than '1'.");
		}

		[Test]
		public void Should_throw_when_value_to_compare_is_null() {
			typeof(ArgumentNullException).ShouldBeThrownBy(() => new LessThanValidator<Person, int>(null));
		}

		[Test]
		public void Extracts_property_from_expression() {
			IComparisonValidator validator = new LessThanValidator<Person, int>(x => x.Id);
			validator.MemberToCompare.ShouldEqual(typeof(Person).GetProperty("Id"));
		}

		[Test]
		public void Extracts_property_from_constant_using_expression() {
			IComparisonValidator validator = new LessThanValidator<Person, int>(x => 2);
			validator.ValueToCompare.ShouldEqual(2);
		}

        [Test]
        public void Extracts_property_from_constant()
        {
            IComparisonValidator validator = FakeLessThanValidator<Person, int>.Validate(2);
            validator.ValueToCompare.ShouldEqual(2);
        }

		[Test]
		public void Comparison_type() {
			validator.Comparison.ShouldEqual(Comparison.LessThan);
		}
	}

    public class FakeLessThanValidator<T, TProperty> where TProperty : IComparable<TProperty>
    {
        public static LessThanValidator<T, TProperty> Validate(TProperty valueToCompare)
        {
            Expression constant = Expression.Constant(valueToCompare, typeof(TProperty));
            ParameterExpression parameter = Expression.Parameter(typeof(T),"t");
            Expression<Func<T, TProperty>> lambda = Expression.Lambda<Func<T, TProperty>>(constant,parameter);

            return new LessThanValidator<T, TProperty>(lambda);
        }
    }
    
}