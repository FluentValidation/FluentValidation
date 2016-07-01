#region License

// Copyright (c) Jeremy Skinner (http://www.jeremyskinner.co.uk)
//  
// Licensed under the Apache License, Version 2.0 (the "License"); 
// You may not use this file except in compliance with the License. 
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
// The latest version of this file can be found at http://github.com/JeremySkinner/FluentValidation

#endregion

namespace FluentValidation.TestHelper {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using Internal;
	using Results;

	internal class TestPropertyChain<TValue, TValue1> : ITestPropertyChain<TValue> {
		private readonly IValidationResultTester validationResultTester;
		private readonly IEnumerable<MemberInfo> properties;

		public TestPropertyChain(IValidationResultTester validationResultTester, IEnumerable<MemberInfo> properties = null) {
			this.validationResultTester = validationResultTester;
			this.properties = properties ?? Enumerable.Empty<MemberInfo>();
		}

		public ITestPropertyChain<TValue2> Property<TValue2>(Expression<Func<TValue, TValue2>> memberAccessor) {
			return new TestPropertyChain<TValue2, TValue1>(validationResultTester, properties.Concat(new[] {((MemberAccessor<TValue, TValue2>) memberAccessor).Member}));
		}

		public IEnumerable<ValidationFailure> ShouldHaveValidationError() {
			return validationResultTester.ShouldHaveValidationError(properties);
		}

		public void ShouldNotHaveValidationError() {
			validationResultTester.ShouldNotHaveValidationError(properties);
		}
	}
}