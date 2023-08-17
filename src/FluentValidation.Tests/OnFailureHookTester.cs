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

using Xunit;

public class OnFailureHookTester {

	[Fact]
	public void Runs_hook_when_failure_created() {
		try {
			ValidatorOptions.Global.OnFailureCreated = (failure, context, propertyValue, rule, component) => {
				failure.PropertyName = "Foo";
				return failure;
			};

			var validator = new InlineValidator<Person>();
			validator.RuleFor(x => x.Surname).NotNull();
			var result = validator.Validate(new Person());
			result.Errors[0].PropertyName.ShouldEqual("Foo");
		}
		finally {
			ValidatorOptions.Global.OnFailureCreated = null;
		}

	}
}
