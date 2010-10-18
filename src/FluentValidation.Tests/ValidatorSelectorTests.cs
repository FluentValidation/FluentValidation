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
	using System;
	using System.Linq;
	using System.Linq.Expressions;
	using Internal;
	using NUnit.Framework;
	using Validators;
	using System.Collections.Generic;

	[TestFixture]
	public class ValidatorSelectorTests {

		[Test]
		public void DefaultValidatorSelector_always_returns_true() {
			var selector = new DefaultValidatorSelector();
			selector.CanExecute<object>(null, null).ShouldBeTrue();
		}

		[Test]
		public void MemberNameValidatorSelector_returns_true_when_property_name_matches() {
			var selector = new MemberNameValidatorSelector(new[] { "SomeProperty" });
			var rule = CreateRule(x => x.SomeProperty);
			selector.CanExecute(rule, "SomeProperty").ShouldBeTrue();
		}

		[Test]
		public void MemberNameValidatorSelector_returns_false_when_property_name_does_not_match() {
			var selector = new MemberNameValidatorSelector(new[] { "SomeProperty" });
			var rule = CreateRule(x => x.SomeOtherProperty);
			selector.CanExecute(rule, "SomeOtherProperty").ShouldBeFalse();
		}

		[Test]
		public void MemberValidatorSelector_returns_true_when_members_match() {
			var selector = new MemberValidatorSelector(new[] { Extensions.GetMember<TestObject, object>(x => x.SomeProperty) });
			var rule = CreateRule(x => x.SomeProperty);
			selector.CanExecute(rule, "SomeProperty").ShouldBeTrue();
		}

		[Test]
		public void MemberValidatorSelector_returns_false_when_members_do_not_match() {
			var selector = new MemberValidatorSelector(new[] { Extensions.GetMember<TestObject, object>(x => x.SomeProperty) });
			var rule = CreateRule(x => x.SomeOtherProperty);
			selector.CanExecute(rule, "SomeOtherProperty").ShouldBeFalse();
		}

		private PropertyRule<TestObject> CreateRule(Expression<Func<TestObject, object>> expression) {
			var rule = PropertyRule<TestObject>.Create(expression);
			rule.AddValidator(new NotNullValidator());
			return rule;
		}

		private class TestObject {
			public object SomeProperty { get; set; }
			public object SomeOtherProperty { get; set; }
		}
	}
}