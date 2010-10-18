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
// The latest version of this file can be found at http://FluentValidation.codeplex.com
#endregion

namespace FluentValidation.Tests {
	using Internal;
	using NUnit.Framework;

	[TestFixture]
	public class InstanceCacheTester {
		InstanceCache cache;

		[SetUp]
		public void Setup() {
			cache = new InstanceCache();
		}

		[Test]
		public void GetInstance_WhenNotCached_CreatesInstance() {
			var result = cache.GetOrCreateInstance(typeof(TestModel));

			result.ShouldBe<TestModel>();
			result.ShouldNotBeNull();
		}

		[Test]
		public void GetInstance_WhenCached_ReUsesInstance() {
			var instance = cache.GetOrCreateInstance(typeof(TestModel));

			var result = cache.GetOrCreateInstance(typeof(TestModel));
			result.ShouldBeTheSameAs(instance);
		}

		private class TestModel {
			
		}
	}
}