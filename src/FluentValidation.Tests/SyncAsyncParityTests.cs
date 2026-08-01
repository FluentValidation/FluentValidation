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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

/// <summary>
/// Guards the core invariant of the Zomp.SyncMethodGenerator source generator: the generated
/// synchronous <c>Validate</c> methods must produce results identical to the hand-written
/// <c>ValidateAsync</c> sources they are generated from. Each test drives each validator
/// through both the sync and async entry points and asserts the two results match exactly.
/// </summary>
public class SyncAsyncParityTests {
	private static async Task AssertParity(IValidator<Person> validator, Person model) {
		var syncResult = validator.Validate(model);
		var asyncResult = await validator.ValidateAsync(model);

		syncResult.IsValid.ShouldEqual(asyncResult.IsValid);
		syncResult.Errors.Count.ShouldEqual(asyncResult.Errors.Count);

		var syncFailures = syncResult.Errors
			.Select(f => (f.PropertyName, f.ErrorCode, f.ErrorMessage))
			.ToList();
		var asyncFailures = asyncResult.Errors
			.Select(f => (f.PropertyName, f.ErrorCode, f.ErrorMessage))
			.ToList();

		// Assert.Equal on lists compares elements in order, so this also pins failure ordering.
		syncFailures.ShouldEqual(asyncFailures);
	}

	[Fact]
	public async Task Property_rule_parity() {
		var validator = new TestValidator();
		validator.RuleFor(x => x.Surname).NotNull().Length(1, 5);

		await AssertParity(validator, new Person()); // fails NotNull (and Length short-circuits on null input)
		await AssertParity(validator, new Person { Surname = "abcdef" }); // fails Length
		await AssertParity(validator, new Person { Surname = "abc" }); // passes
	}

	[Fact]
	public async Task Cascade_stop_parity() {
		var validator = new TestValidator();
		validator.RuleFor(x => x.Surname).Cascade(CascadeMode.Stop).NotNull().Length(1, 5);

		// Only the first (NotNull) failure should surface in both paths.
		await AssertParity(validator, new Person());
		await AssertParity(validator, new Person { Surname = "abcdef" });
	}

	[Fact]
	public async Task Collection_rule_parity() {
		var validator = new TestValidator();
		validator.RuleForEach(x => x.NickNames).NotNull();

		// Indexed property names must be inferred identically.
		await AssertParity(validator, new Person { NickNames = ["a", null, "c", null] });
		await AssertParity(validator, new Person { NickNames = ["a", "b"] });
	}

	[Fact]
	public async Task Include_rule_parity() {
		var included = new TestValidator();
		included.RuleFor(x => x.Forename).NotNull();

		var validator = new TestValidator();
		validator.Include(included);
		validator.RuleFor(x => x.Surname).NotNull();

		await AssertParity(validator, new Person());
		await AssertParity(validator, new Person { Forename = "f", Surname = "s" });
	}

	[Fact]
	public async Task Child_validator_collection_parity() {
		var childValidator = new TestValidator();
		childValidator.RuleFor(x => x.Forename).NotNull();

		var validator = new TestValidator();
		validator.RuleForEach(x => x.Children).SetValidator(childValidator);

		await AssertParity(validator, new Person {
			Children = new List<Person> { new(), new() { Forename = "ok" } }
		});
	}

	[Fact]
	public async Task Dependent_rules_parity() {
		var validator = new TestValidator();
		validator.RuleFor(x => x.Surname).NotNull()
			.DependentRules(() => {
				validator.RuleFor(x => x.Forename).NotNull();
			});

		// Surname null => dependent Forename rule must NOT run in either path.
		await AssertParity(validator, new Person());
		// Surname set => dependent Forename rule runs and fails in both paths.
		await AssertParity(validator, new Person { Surname = "x" });
	}
}
