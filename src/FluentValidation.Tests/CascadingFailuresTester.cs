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
#pragma warning disable 618
#pragma warning disable 1998

namespace FluentValidation.Tests;

using System;
using System.Threading.Tasks;
using Xunit;

public class CascadingFailuresTester : IDisposable {
	TestValidator _validator;

	public CascadingFailuresTester() {
		SetBothGlobalCascadeModes(CascadeMode.Continue);
		_validator = new TestValidator();
	}

	public void Dispose() {
		SetBothGlobalCascadeModes(CascadeMode.Continue);
	}

	[Fact]
	public void Validation_continues_on_failure() {
		_validator.RuleFor(x => x.Surname).NotNull().Equal("Foo");
		var results = _validator.Validate(new Person());
		results.Errors.Count.ShouldEqual(2);
	}

	[Fact]
	public void Validation_stops_on_first_rule_level_failure() {
		SetBothGlobalCascadeModes(CascadeMode.Stop);

		_validator.RuleFor(x => x.Surname).NotNull().Equal("Foo");
		var results = _validator.Validate(new Person());
		results.Errors.Count.ShouldEqual(1);
	}

	[Fact]
	public void Validation_stops_on_first_rule_level_failure_and_evaluates_other_rules_when_globaldefault_rule_Stop() {
		ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;

		_validator.RuleFor(x => x.Surname).NotNull().Equal("Foo");
		_validator.RuleFor(x => x.Forename).NotNull().Equal("Foo");
		_validator.RuleFor(x => x.Email).NotNull().Equal("Foo");

		var results = _validator.Validate(new Person());
		results.Errors.Count.ShouldEqual(3);
	}

	[Fact]
	public void Validation_stops_after_first_rule_failure_when_globaldefault_class_Stop() {
		ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Stop;

		_validator.RuleFor(x => x.Surname).NotNull().Equal("Foo");
		_validator.RuleFor(x => x.Forename).NotNull().Equal("Foo");
		_validator.RuleFor(x => x.Email).NotNull().Equal("Foo");

		var results = _validator.Validate(new Person());
		results.Errors.Count.ShouldEqual(2);
	}

	[Fact]
	public void Validation_continues_on_failure_when_globaldefault_both_Stop_and_ruleleveloverride_Continue() {
		SetBothGlobalCascadeModes(CascadeMode.Stop);

		_validator.RuleFor(x => x.Surname).Cascade(CascadeMode.Continue).NotNull().Equal("Foo");
		var results = _validator.Validate(new Person());
		results.Errors.Count.ShouldEqual(2);
	}

	[Fact]
	public void Validation_continues_on_failure_when_globaldefault_rule_stop_and_ruleleveloverride_Continue() {
		ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;

		_validator.RuleFor(x => x.Surname).Cascade(CascadeMode.Continue).NotNull().Equal("Foo");
		_validator.RuleFor(x => x.Forename).Cascade(CascadeMode.Continue).NotNull().Equal("Foo");
		_validator.RuleFor(x => x.Email).Cascade(CascadeMode.Continue).NotNull().Equal("Foo");

		var results = _validator.Validate(new Person());
		results.Errors.Count.ShouldEqual(6);
	}

	[Fact]
	public void Validation_stops_after_first_rule_failure_when_globaldefault_class_stop_and_ruleleveloverride_Continue() {
		ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Stop;

		_validator.RuleFor(x => x.Surname).Cascade(CascadeMode.Continue).NotNull().Equal("Foo");
		_validator.RuleFor(x => x.Forename).Cascade(CascadeMode.Continue).NotNull().Equal("Foo");
		_validator.RuleFor(x => x.Email).Cascade(CascadeMode.Continue).NotNull().Equal("Foo");

		var results = _validator.Validate(new Person());
		results.Errors.Count.ShouldEqual(2);
	}

	[Fact]
	public void Validation_stops_on_first_failure_when_globaldefault_both_Continue_and_ruleleveloverride_Stop() {
		SetBothGlobalCascadeModes(CascadeMode.Continue);
		_validator.RuleFor(x => x.Surname).Cascade(CascadeMode.Stop).NotNull().Equal("Foo");
		var results = _validator.Validate(new Person());
		results.Errors.Count.ShouldEqual(1);
	}

	[Fact]
	public void Validation_continues_to_second_validator_when_first_validator_succeeds_and_globaldefault_both_Stop() {
		SetBothGlobalCascadeModes(CascadeMode.Stop);
		_validator.RuleFor(x => x.Surname).NotNull().Length(2, 10);
		var result = _validator.Validate(new Person() {Surname = "x"});
		result.IsValid.ShouldBeFalse();
	}

	[Fact]
	public void Validation_continues_to_first_failing_validator_then_stops_in_all_rules_when_first_validator_succeeds_and_globaldefault_rule_Stop() {
		ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;

		_validator.RuleFor(x => x.Surname).NotNull().Equal("Foo").Length(2, 10);
		_validator.RuleFor(x => x.Forename).NotNull().Equal("Foo").Length(2, 10);
		_validator.RuleFor(x => x.Email).NotNull().Equal("Foo").Length(2, 10);
		_validator.RuleFor(x => x.CreditCard).NotNull().Equal("Foo").Length(2, 10);

		var result = _validator.Validate(new Person() { Surname = "x", Forename = "x", Email = "x", CreditCard = "x" });

		result.Errors.Count.ShouldEqual(4);
	}

	[Fact]
	public void Validation_stops_after_first_rule_when_first_rule_fails_and_globaldefault_class_Stop() {
		ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Stop;

		_validator.RuleFor(x => x.Surname).NotNull().Equal("Foo").Length(2, 10);
		_validator.RuleFor(x => x.Forename).NotNull().Equal("Foo").Length(2, 10);
		_validator.RuleFor(x => x.Email).NotNull().Equal("Foo").Length(2, 10);
		_validator.RuleFor(x => x.CreditCard).NotNull().Equal("Foo").Length(2, 10);

		var result = _validator.Validate(new Person() { Surname = "x", Forename = "x", Email = "x", CreditCard = "x" });

		result.Errors.Count.ShouldEqual(2);
	}

	[Fact]
	public void Validation_stops_on_first_failure_when_classlevel_Stop_and_ruleleveldefault_Stop() {
		SetBothValidatorCascadeModes(CascadeMode.Stop);

		_validator.RuleFor(x => x.Surname).NotNull().Equal("Foo");
		_validator.RuleFor(x => x.Surname).NotNull().Equal("Foo");
		var results = _validator.Validate(new Person());
		results.Errors.Count.ShouldEqual(1);
	}

	[Fact]
	public void Validation_stops_on_first_failure_when_ruleleveldefault_Stop() {
		_validator.RuleLevelCascadeMode = CascadeMode.Stop;

		_validator.RuleFor(x => x.Surname).NotNull().Equal("Foo");
		var results = _validator.Validate(new Person());
		results.Errors.Count.ShouldEqual(1);
	}

	[Fact]
	public void Validation_continues_when_classlevel_Continue_and_ruleleveldefault_Continue() {
		SetBothValidatorCascadeModes(CascadeMode.Continue);

		_validator.RuleFor(x => x.Surname).NotNull().Equal("Foo");
		var results = _validator.Validate(new Person());
		results.Errors.Count.ShouldEqual(2);
	}

	[Fact]
	public void Validation_continues_on_failure_when_classlevel_Stop_and_ruleleveldefault_Stop_and_ruleleveloverride_Continue() {
		SetBothValidatorCascadeModes(CascadeMode.Stop);

		_validator.RuleFor(x => x.Surname).Cascade(CascadeMode.Continue).NotNull().Equal("Foo");
		var results = _validator.Validate(new Person());
		results.Errors.Count.ShouldEqual(2);
	}

	[Fact]
	public void Validation_continues_on_failure_when_ruleleveldefault_Stop_and_ruleleveloverride_Continue() {
		_validator.RuleLevelCascadeMode = CascadeMode.Stop;

		_validator.RuleFor(x => x.Surname).Cascade(CascadeMode.Continue).NotNull().Equal("Foo");
		var results = _validator.Validate(new Person());
		results.Errors.Count.ShouldEqual(2);
	}

	[Fact]
	public void Validation_stops_on_failure_when_classlevel_Continue_and_ruleleveldefault_Continue_and_ruleleveloverride_Stop() {
		SetBothValidatorCascadeModes(CascadeMode.Continue);

		_validator.RuleFor(x => x.Surname).Cascade(CascadeMode.Stop).NotNull().Equal("Foo");
		var results = _validator.Validate(new Person());
		results.Errors.Count.ShouldEqual(1);
	}

	[Fact]
	public void Cascade_mode_can_be_set_after_validator_instantiated() {
		_validator.RuleFor(x => x.Surname).NotNull().Equal("Foo");
		SetBothValidatorCascadeModes(CascadeMode.Stop);
		var results = _validator.Validate(new Person());
		results.Errors.Count.ShouldEqual(1);
	}

	[Fact]
	public void Cascade_mode_can_be_set_after_validator_instantiated_legacy() {
		_validator.RuleFor(x => x.Surname).NotNull().Equal("Foo");
		_validator.RuleLevelCascadeMode = CascadeMode.Stop;
		var results = _validator.Validate(new Person());
		results.Errors.Count.ShouldEqual(1);
	}

	[Fact]
	public async Task Validation_continues_on_failure_async() {
		_validator.RuleFor(x => x.Surname).MustAsync(async (x, c) => x != null).MustAsync(async (x, c) => x == "foo");
		var results = await _validator.ValidateAsync(new Person());
		results.Errors.Count.ShouldEqual(2);
	}

	[Fact]
	public async Task Validation_stops_on_first_failure_async() {
		SetBothGlobalCascadeModes(CascadeMode.Stop);

		_validator.RuleFor(x => x.Surname).MustAsync(async (x, c) => x != null).MustAsync(async (x, c) => x == "foo");
		var results = await _validator.ValidateAsync(new Person());
		results.Errors.Count.ShouldEqual(1);
	}

	[Fact]
	public async Task Validation_stops_on_first_rule_level_failure_when_globaldefault_rule_Stop_async() {
		ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;

		_validator.RuleFor(x => x.Surname).MustAsync(async (x, c) => x != null).MustAsync(async (x, c) => x == "foo");
		_validator.RuleFor(x => x.Forename).MustAsync(async (x, c) => x != null).MustAsync(async (x, c) => x == "foo");
		_validator.RuleFor(x => x.Email).MustAsync(async (x, c) => x != null).MustAsync(async (x, c) => x == "foo");

		var results = await _validator.ValidateAsync(new Person());
		results.Errors.Count.ShouldEqual(3);
	}

	[Fact]
	public async Task Validation_stops_after_first_rule_failure_when_globaldefault_class_Stop_async() {
		ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Stop;

		_validator.RuleFor(x => x.Surname).MustAsync(async (x, c) => x != null).MustAsync(async (x, c) => x == "foo");
		_validator.RuleFor(x => x.Forename).MustAsync(async (x, c) => x != null).MustAsync(async (x, c) => x == "foo");
		_validator.RuleFor(x => x.Email).MustAsync(async (x, c) => x != null).MustAsync(async (x, c) => x == "foo");
		var results = await _validator.ValidateAsync(new Person());
		results.Errors.Count.ShouldEqual(2);
	}

	[Fact]
	public async Task Validation_continues_on_failure_when_globaldefault_both_Stop_and_ruleleveloverride_Continue_async() {
		SetBothGlobalCascadeModes(CascadeMode.Stop);

		_validator.RuleFor(x => x.Surname).Cascade(CascadeMode.Continue).MustAsync(async (x, c) => x != null).MustAsync(async (x, c) => x == "foo");
		var results = await _validator.ValidateAsync(new Person());
		results.Errors.Count.ShouldEqual(2);
	}

	[Fact]
	public async Task Validation_continues_on_failure_when_globaldefault_rule_stop_and_ruleleveloverride_Continue_async() {
		ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;

		_validator.RuleFor(x => x.Surname).Cascade(CascadeMode.Continue).MustAsync(async (x, c) => x != null).MustAsync(async (x, c) => x == "foo");
		_validator.RuleFor(x => x.Forename).Cascade(CascadeMode.Continue).MustAsync(async (x, c) => x != null).MustAsync(async (x, c) => x == "foo");
		_validator.RuleFor(x => x.Email).Cascade(CascadeMode.Continue).MustAsync(async (x, c) => x != null).MustAsync(async (x, c) => x == "foo");

		var results = await _validator.ValidateAsync(new Person());
		results.Errors.Count.ShouldEqual(6);
	}

	[Fact]
	public async Task Validation_stops_after_first_rule_failure_when_globaldefault_class_stop_and_ruleleveloverride_Continue_async() {
		ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Stop;

		_validator.RuleFor(x => x.Surname).Cascade(CascadeMode.Continue).MustAsync(async (x, c) => x != null).MustAsync(async (x, c) => x == "foo");
		_validator.RuleFor(x => x.Forename).Cascade(CascadeMode.Continue).MustAsync(async (x, c) => x != null).MustAsync(async (x, c) => x == "foo");
		_validator.RuleFor(x => x.Email).Cascade(CascadeMode.Continue).MustAsync(async (x, c) => x != null).MustAsync(async (x, c) => x == "foo");

		var results = await _validator.ValidateAsync(new Person());
		results.Errors.Count.ShouldEqual(2);
	}

	[Fact]
	public async Task Validation_stops_on_first_Failure_when_globaldefault_both_Continue_and_ruleleveloverride_Stop_async() {
		SetBothGlobalCascadeModes(CascadeMode.Continue);
		_validator.RuleFor(x => x.Surname).Cascade(CascadeMode.Stop).MustAsync(async (x, c) => x != null).MustAsync(async (x, c) => x == "foo");
		var results = await _validator.ValidateAsync(new Person());
		results.Errors.Count.ShouldEqual(1);
	}

	[Fact]
	public async Task Validation_stops_on_first_Failure_when_globaldefault_both_Continue_and_ruleleveloverride_Stop_async_and_async_validator_is_invoked_synchronously() {
		SetBothGlobalCascadeModes(CascadeMode.Continue);
		_validator.RuleFor(x => x.Surname).Cascade(CascadeMode.Stop).NotNull().Equal("Foo");
		var results = await _validator.ValidateAsync(new Person());
		results.Errors.Count.ShouldEqual(1);
	}

	[Fact]
	public async Task Validation_continues_to_second_validator_when_first_validator_succeeds_and_globaldefault_both_Stop_async() {
		SetBothGlobalCascadeModes(CascadeMode.Stop);
		_validator.RuleFor(x => x.Surname).MustAsync(async (x, c) => x != null).MustAsync(async (x, c) => x == "foo");
		var result = await _validator.ValidateAsync(new Person { Surname = "x" });
		result.IsValid.ShouldBeFalse();
	}

	[Fact]
	public async Task Validation_continues_to_first_failing_validator_then_stops_in_all_rules_when_first_validator_succeeds_and_globaldefault_rule_Stop_async() {
		ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;

		_validator.RuleFor(x => x.Surname)
			.MustAsync(async (x, c) => x != null)
			.MustAsync(async (x, c) => x.Length >= 2)
			.MustAsync(async (x, c) => x == "foo");

		_validator.RuleFor(x => x.Forename)
			.MustAsync(async (x, c) => x != null)
			.MustAsync(async (x, c) => x.Length >= 2)
			.MustAsync(async (x, c) => x == "foo");

		_validator.RuleFor(x => x.Email)
			.MustAsync(async (x, c) => x != null)
			.MustAsync(async (x, c) => x.Length >= 2)
			.MustAsync(async (x, c) => x == "foo");

		_validator.RuleFor(x => x.CreditCard)
			.MustAsync(async (x, c) => x != null)
			.MustAsync(async (x, c) => x.Length >= 2)
			.MustAsync(async (x, c) => x == "foo");

		var result = await _validator.ValidateAsync(new Person { Surname = "x", Forename = "x", Email = "x", CreditCard = "x" });
		result.Errors.Count.ShouldEqual(4);
	}

	[Fact]
	public async Task Validation_stops_after_first_rule_when_first_rule_fails_and_globaldefault_class_Stop_async() {
		ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Stop;

		_validator.RuleFor(x => x.Surname)
			.MustAsync(async (x, c) => x != null)
			.MustAsync(async (x, c) => x.Length >= 2)
			.MustAsync(async (x, c) => x == "foo");

		_validator.RuleFor(x => x.Forename)
			.MustAsync(async (x, c) => x != null)
			.MustAsync(async (x, c) => x.Length >= 2)
			.MustAsync(async (x, c) => x == "foo");

		_validator.RuleFor(x => x.Email)
			.MustAsync(async (x, c) => x != null)
			.MustAsync(async (x, c) => x.Length >= 2)
			.MustAsync(async (x, c) => x == "foo");

		_validator.RuleFor(x => x.CreditCard)
			.MustAsync(async (x, c) => x != null)
			.MustAsync(async (x, c) => x.Length >= 2)
			.MustAsync(async (x, c) => x == "foo");

		var result = await _validator.ValidateAsync(new Person { Surname = "x", Forename = "x", Email = "x", CreditCard = "x" });
		result.Errors.Count.ShouldEqual(2);
	}

	[Fact]
	public async Task Validation_stops_on_first_failure_when_classlevel_Stop_and_ruleleveldefault_Stop_async() {
		SetBothValidatorCascadeModes(CascadeMode.Stop);

		_validator.RuleFor(x => x.Surname).MustAsync(async (x, c) => x != null).MustAsync(async (x, c) => x == "foo");
		_validator.RuleFor(x => x.Surname).MustAsync(async (x, c) => x != null).MustAsync(async (x, c) => x == "foo");
		var results = await _validator.ValidateAsync(new Person());
		results.Errors.Count.ShouldEqual(1);
	}

	[Fact]
	public async Task Validation_stops_on_first_failure_when_ruleleveldefault_Stop_async() {
		_validator.RuleLevelCascadeMode = CascadeMode.Stop;

		_validator.RuleFor(x => x.Surname).MustAsync(async (x, c) => x != null).MustAsync(async (x, c) => x == "foo");
		var results = await _validator.ValidateAsync(new Person());
		results.Errors.Count.ShouldEqual(1);
	}

	[Fact]
	public async Task Validation_continues_when_classlevel_Continue_and_ruleleveldefault_Continue_async() {
		SetBothValidatorCascadeModes(CascadeMode.Continue);

		_validator.RuleFor(x => x.Surname).MustAsync(async (x, c) => x != null).MustAsync(async (x, c) => x == "foo");
		var results = await _validator.ValidateAsync(new Person());
		results.Errors.Count.ShouldEqual(2);
	}

	[Fact]
	public async Task Validation_continues_on_failure_when_classlevel_Stop_and_ruleleveldefault_Stop_and_ruleleveloverride_Continue_async() {
		SetBothValidatorCascadeModes(CascadeMode.Stop);

		_validator.RuleFor(x => x.Surname).Cascade(CascadeMode.Continue).MustAsync(async (x, c) => x != null).MustAsync(async (x, c) => x == "foo");
		var results = await _validator.ValidateAsync(new Person());
		results.Errors.Count.ShouldEqual(2);
	}

	[Fact]
	public async Task Validation_continues_on_failure_when_ruleleveldefault_Stop_and_ruleleveloverride_Continue_async() {
		_validator.RuleLevelCascadeMode = CascadeMode.Stop;

		_validator.RuleFor(x => x.Surname).Cascade(CascadeMode.Continue).MustAsync(async (x, c) => x != null).MustAsync(async (x, c) => x == "foo");
		var results = await _validator.ValidateAsync(new Person());
		results.Errors.Count.ShouldEqual(2);
	}

	[Fact]
	public async Task Validation_stops_on_failure_when_classlevel_Continue_and_ruleleveldefault_Continue_and_ruleleveloverride_Stop_async() {
		SetBothValidatorCascadeModes(CascadeMode.Continue);

		_validator.RuleFor(x => x.Surname).Cascade(CascadeMode.Stop).MustAsync(async (x, c) => x != null).MustAsync(async (x, c) => x == "foo");
		var results = await _validator.ValidateAsync(new Person());
		results.Errors.Count.ShouldEqual(1);
	}

	[Fact]
	public async Task Cascade_mode_can_be_set_after_validator_instantiated_async() {
		_validator.RuleFor(x => x.Surname).MustAsync(async (x, c) => x != null).MustAsync(async (x, c) => x == "foo");
		SetBothValidatorCascadeModes(CascadeMode.Stop);
		var results = await _validator.ValidateAsync(new Person());
		results.Errors.Count.ShouldEqual(1);
	}

	[Fact]
	public async Task Cascade_mode_can_be_set_after_validator_instantiated_async_legacy() {
		_validator.RuleFor(x => x.Surname).MustAsync(async (x, c) => x != null).MustAsync(async (x, c) => x == "foo");
		_validator.RuleLevelCascadeMode = CascadeMode.Stop;
		var results = await _validator.ValidateAsync(new Person());
		results.Errors.Count.ShouldEqual(1);
	}

	[Fact]
	public void CascadeMode_values_should_correspond_to_correct_integers() {
		// 12.0 removed the "StopOnFirstFailure" option which was value 1.
		// For compatibility, "Stop" should still equate to 2, rather than being renumbered to 1.
		Assert.Equal(0, (int)CascadeMode.Continue);
		Assert.Equal(2, (int)CascadeMode.Stop);
	}

	private void SetBothValidatorCascadeModes(CascadeMode cascadeMode) {
		_validator.ClassLevelCascadeMode = cascadeMode;
		_validator.RuleLevelCascadeMode = cascadeMode;
	}

	private static void SetBothGlobalCascadeModes(CascadeMode cascadeMode) {
		ValidatorOptions.Global.DefaultClassLevelCascadeMode = cascadeMode;
		ValidatorOptions.Global.DefaultRuleLevelCascadeMode = cascadeMode;
	}
}
