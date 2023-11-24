#region License
// Copyright (c) .NET Foundation and contributors.
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
// The latest version of this file can be found at https://github.com/FluentValidation/FluentValidation
#endregion

namespace FluentValidation.Tests;

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

public class NullTester {
	public NullTester() {
		CultureScope.SetDefaultCulture();
	}

	[Fact]
	public void NullValidator_should_fail_if_value_has_value() {
		var validator = new TestValidator(v => v.RuleFor(x => x.Surname).Null());
		var result = validator.Validate(new Person{Surname = "Foo"});
		result.IsValid.ShouldBeFalse();
	}

	[Fact]
	public void NullValidator_should_pass_if_value_is_null() {
		var validator = new TestValidator(v => v.RuleFor(x => x.Surname).Null());
		var result = validator.Validate(new Person { Surname = null });
		result.IsValid.ShouldBeTrue();
	}

	[Fact]
	public void When_the_validator_passes_the_error_message_should_be_set() {
		var validator = new TestValidator(v => v.RuleFor(x => x.Surname).Null());
		var result = validator.Validate(new Person { Surname = "Foo" });
		result.Errors.Single().ErrorMessage.ShouldEqual("'Surname' must be empty.");
	}

	[Fact]
	public void Not_null_validator_should_not_crash_with_non_nullable_value_type() {
		var validator = new TestValidator(v => v.RuleFor(x => x.Id).Null());
		var result = validator.Validate(new Person());
		result.IsValid.ShouldBeFalse();
	}

	[Fact]
	public void Passes_when_nullable_value_type_is_null() {
		var validator = new TestValidator(v => v.RuleFor(x => x.NullableInt).Null());
		var result = validator.Validate(new Person());
		result.IsValid.ShouldBeTrue();
	}

	[Fact]
	public void NullProperty_should_throw_NullReferenceException() {
		var validator = new InlineValidator<Person>();
		validator.RuleFor(x => x.Orders.Count).NotEmpty();

		var ex = Assert.Throws<NullReferenceException>(() => validator.Validate(new Person {
			Orders = null
		}));

		ex.Message.ShouldEqual("NullReferenceException occurred when executing rule for x => x.Orders.Count. If this property can be null you should add a null check using a When condition");
		ex.InnerException.ShouldNotBeNull();
		ex.InnerException!.GetType().ShouldEqual(typeof(NullReferenceException));
	}

	[Fact]
	public void ForEachNullProperty_should_throw_NullReferenceException_when_exception_occurs() {
		var validator = new InlineValidator<Person>();
		validator.RuleForEach(x => x.Orders[0].Payments).NotNull();

		var ex = Assert.Throws<NullReferenceException>(() => validator.Validate(new Person {
			Orders = null
		}));
		ex.Message.ShouldEqual("NullReferenceException occurred when executing rule for x => x.Orders.get_Item(0).Payments. If this property can be null you should add a null check using a When condition");
		ex.InnerException.ShouldNotBeNull();
		ex.InnerException!.GetType().ShouldEqual(typeof(NullReferenceException));
	}
}