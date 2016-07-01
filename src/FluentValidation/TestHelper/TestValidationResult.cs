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
	using System.Linq;
	using System.Reflection;
	using Internal;
	using Results;

#pragma warning disable 1591
	public class TestValidationResult<T, TValue> where T : class {
		public ValidationResult Result { get; private set; }
		public MemberAccessor<T, TValue> MemberAccessor { get; private set; }

		public TestValidationResult(ValidationResult validationResult, MemberAccessor<T, TValue> memberAccessor) {
			Result = validationResult;
			MemberAccessor = memberAccessor;
		}

		public ITestPropertyChain<TValue> Which {
			get {
				var resultTester = new ValidationResultTester<T, TValue>(this);
				return new TestPropertyChain<TValue, TValue>(resultTester, Enumerable.Empty<MemberInfo>());
			}
		}
	}
#pragma warning restore 1591

}