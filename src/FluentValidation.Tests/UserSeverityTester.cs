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
	using System.Linq;
	using Xunit;

	
	public class UserSeerityTester {
		TestValidator validator;


		public UserSeerityTester()
		{
			validator = new TestValidator();
		}

		[Fact]
		public void Stores_user_severity_against_validation_failure() {
			validator.RuleFor(x => x.Surname).NotNull().WithSeverity(Severity.Info);
			var result = validator.Validate(new Person());
			result.Errors.Single().Severity.ShouldEqual(Severity.Info);
		}

		[Fact]
		public void Defaults_user_severity_to_error() {
		    validator.RuleFor( x => x.Surname ).NotNull();
		    var result = validator.Validate( new Person() );
		    result.Errors.Single().Severity.ShouldEqual( Severity.Error );
		}
	}
}