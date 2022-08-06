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
using Xunit;

public class CascadeModePropertiesTesterLegacy : IDisposable {
	TestValidator _validator;

	public CascadeModePropertiesTesterLegacy() {
		SetBothGlobalCascadeModes(CascadeMode.Continue);
		_validator = new TestValidator();
	}

	public void Dispose() {
		SetBothGlobalCascadeModes(CascadeMode.Continue);
	}

	[Fact]
	public void Setting_global_default_CascadeMode_Stop_sets_both_rule_and_class_level_global_default_properties() {
		ValidatorOptions.Global.CascadeMode = CascadeMode.Stop;

		Assert.Equal(CascadeMode.Stop, ValidatorOptions.Global.DefaultRuleLevelCascadeMode);
		Assert.Equal(CascadeMode.Stop, ValidatorOptions.Global.DefaultClassLevelCascadeMode);
	}

	[Fact]
	public void Setting_global_default_CascadeMode_Continue_sets_both_rule_and_class_level_global_default_properties() {
		ValidatorOptions.Global.CascadeMode = CascadeMode.Stop;
		ValidatorOptions.Global.CascadeMode = CascadeMode.Continue;

		Assert.Equal(CascadeMode.Continue, ValidatorOptions.Global.DefaultRuleLevelCascadeMode);
		Assert.Equal(CascadeMode.Continue, ValidatorOptions.Global.DefaultClassLevelCascadeMode);
	}

	[Fact]
	public void Setting_global_default_CascadeMode_StopOnFirstFailure_sets_rule_Stop_and_class_Continue() {
		ValidatorOptions.Global.CascadeMode = CascadeMode.StopOnFirstFailure;

		Assert.Equal(CascadeMode.Stop, ValidatorOptions.Global.DefaultRuleLevelCascadeMode);
		Assert.Equal(CascadeMode.Continue, ValidatorOptions.Global.DefaultClassLevelCascadeMode);
	}

	[Fact]
	public void Setting_class_CascadeMode_Stop_sets_both_rule_and_class_level_properties() {
		_validator.CascadeMode = CascadeMode.Stop;

		Assert.Equal(CascadeMode.Stop, _validator.RuleLevelCascadeMode);
		Assert.Equal(CascadeMode.Stop, _validator.ClassLevelCascadeMode);
	}

	[Fact]
	public void Setting_class_CascadeMode_Continue_sets_both_rule_and_class_level_properties() {
		_validator.CascadeMode = CascadeMode.Stop;
		_validator.CascadeMode = CascadeMode.Continue;

		Assert.Equal(CascadeMode.Continue, _validator.RuleLevelCascadeMode);
		Assert.Equal(CascadeMode.Continue, _validator.ClassLevelCascadeMode);
	}

	[Fact]
	public void Setting_class_CascadeMode_StopOnFirstFailure_sets_rule_Stop_and_class_Continue() {
		_validator.CascadeMode = CascadeMode.StopOnFirstFailure;

		Assert.Equal(CascadeMode.Stop, _validator.RuleLevelCascadeMode);
		Assert.Equal(CascadeMode.Continue, _validator.ClassLevelCascadeMode);
	}

	[Fact]
	public void Setting_global_DefaultRuleLevelCascadeMode_to_StopOnFirstFailure_sets_Stop() {
		ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.StopOnFirstFailure;

		Assert.Equal(CascadeMode.Stop, ValidatorOptions.Global.DefaultRuleLevelCascadeMode);
	}

	[Fact]
	public void Setting_global_DefaultClassLevelCascadeMode_to_StopOnFirstFailure_sets_Stop() {
		ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.StopOnFirstFailure;

		Assert.Equal(CascadeMode.Stop, ValidatorOptions.Global.DefaultClassLevelCascadeMode);
	}

	[Fact]
	public void Setting_class_RuleLevelCascadeMode_to_StopOnFirstFailure_sets_Stop() {
		_validator.RuleLevelCascadeMode = CascadeMode.StopOnFirstFailure;

		Assert.Equal(CascadeMode.Stop, _validator.RuleLevelCascadeMode);
	}

	[Fact]
	public void Setting_class_ClassLevelCascadeMode_to_StopOnFirstFailure_sets_Stop() {
		_validator.ClassLevelCascadeMode = CascadeMode.StopOnFirstFailure;

		Assert.Equal(CascadeMode.Stop, _validator.ClassLevelCascadeMode);
	}

	[Fact]
	public void Global_default_CascadeMode_Get_returns_Stop_when_both_Stop() {
		SetBothGlobalCascadeModes(CascadeMode.Stop);

		Assert.Equal(CascadeMode.Stop, ValidatorOptions.Global.CascadeMode);
	}

	[Fact]
	public void Global_default_CascadeMode_Get_returns_Continue_when_both_Continue() {
		SetBothGlobalCascadeModes(CascadeMode.Stop);
		SetBothGlobalCascadeModes(CascadeMode.Continue);

		Assert.Equal(CascadeMode.Continue, ValidatorOptions.Global.CascadeMode);
	}

	[Fact]
	public void Global_default_CascadeMode_Get_returns_StopOnFirstFailure_when_class_Continue_and_rule_Stop() {
		ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Continue;
		ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;

		Assert.Equal(CascadeMode.StopOnFirstFailure, ValidatorOptions.Global.CascadeMode);
	}

	[Fact]
	public void Global_default_CascadeMode_Get_throws_exception_when_class_Stop_and_rule_Continue() {
		ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Stop;
		ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Continue;

		Assert.ThrowsAny<Exception>(() => ValidatorOptions.Global.CascadeMode);
	}

	[Fact]
	public void Class_CascadeMode_Get_returns_Stop_when_both_Stop() {
		SetBothValidatorCascadeModes(CascadeMode.Stop);

		Assert.Equal(CascadeMode.Stop, _validator.CascadeMode);
	}

	[Fact]
	public void Class_CascadeMode_Get_returns_Continue_when_both_Continue() {
		SetBothValidatorCascadeModes(CascadeMode.Stop);
		SetBothValidatorCascadeModes(CascadeMode.Continue);

		Assert.Equal(CascadeMode.Continue, _validator.CascadeMode);
	}

	[Fact]
	public void Class_CascadeMode_Get_returns_StopOnFirstFailure_when_class_Continue_and_rule_Stop() {
		_validator.ClassLevelCascadeMode = CascadeMode.Continue;
		_validator.RuleLevelCascadeMode = CascadeMode.Stop;

		Assert.Equal(CascadeMode.StopOnFirstFailure, _validator.CascadeMode);
	}

	[Fact]
	public void Class_CascadeMode_Get_throws_exception_when_class_Stop_and_rule_Continue() {
		_validator.ClassLevelCascadeMode = CascadeMode.Stop;
		_validator.RuleLevelCascadeMode = CascadeMode.Continue;

		Assert.ThrowsAny<Exception>(() => _validator.CascadeMode);
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
