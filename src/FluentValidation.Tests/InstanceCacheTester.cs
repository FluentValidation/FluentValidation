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
// The latest version of this file can be found at https://github.com/JeremySkinner/FluentValidation
#endregion

namespace FluentValidation.Tests {
	using Internal;
	using Xunit;

	
	public class InstanceCacheTester {
		InstanceCache cache;
		public InstanceCacheTester() {
			cache = new InstanceCache();
		}

		[Fact]
		public void GetInstance_WhenNotCached_CreatesInstance() {
			var result = cache.GetOrCreateInstance(typeof(TestModel));

			result.ShouldBe<TestModel>();
			result.ShouldNotBeNull();
		}

		[Fact]
		public void GetInstance_WhenCached_ReUsesInstance() {
			var instance = cache.GetOrCreateInstance(typeof(TestModel));

			var result = cache.GetOrCreateInstance(typeof(TestModel));
			result.ShouldBeTheSameAs(instance);
		}

		private class TestModel {
			
		}
	}
}