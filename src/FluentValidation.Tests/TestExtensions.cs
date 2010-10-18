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
	using System.Collections.Generic;
	using System.Linq;
	using NUnit.Framework;
	using NUnit.Framework.Constraints;
	using Results;

	//Inspired by SpecUnit's SpecificationExtensions
	//http://code.google.com/p/specunit-net/source/browse/trunk/src/SpecUnit/SpecificationExtensions.cs
	public static class TestExtensions {
		public static void ShouldEqual(this object actual, object expected) {
			Assert.AreEqual(expected, actual);
		}

		public static void ShouldBeTheSameAs(this object actual, object expected) {
			Assert.AreSame(expected, actual);
		}

		public static void ShouldBeNull(this object actual) {
			Assert.IsNull(actual);
		}

		public static void ShouldNotBeNull(this object actual) {
			Assert.IsNotNull(actual);
		}

		public static void ShouldBeTrue(this bool b) {
			Assert.IsTrue(b);
		}

		public static void ShouldBeFalse(this bool b) {
			Assert.IsFalse(b);
		}

		public static Exception ShouldBeThrownBy(this Type exceptionType, TestDelegate code) {
			return Assert.Throws(exceptionType, code);
		}

		public static T ShouldBe<T>(this object actual) {
			Assert.IsInstanceOf<T>(actual);
			return (T)actual;
		}

		public static bool IsValid(this IEnumerable<ValidationFailure> errors) {
			return errors.Count() == 0;
		}

		public static void ShouldStartWith(this object actual, string expected) {
			Assert.That(actual, new StartsWithConstraint(expected));
		}
	}
}