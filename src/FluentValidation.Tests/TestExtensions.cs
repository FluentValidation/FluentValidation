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

//Inspired by SpecUnit's SpecificationExtensions
//http://code.google.com/p/specunit-net/source/browse/trunk/src/SpecUnit/SpecificationExtensions.cs
public static class TestExtensions {
	public static void ShouldEqual(this object actual, object expected) {
		Assert.Equal(expected, actual);
	}

	public static void ShouldBeTheSameAs(this object actual, object expected) {
		Assert.Same(expected, actual);
	}

	public static void ShouldBeNull(this object actual) {
		Assert.Null(actual);
	}

	public static void ShouldNotBeNull(this object actual) {
		Assert.NotNull(actual);
	}

	public static void ShouldBeTrue(this bool b) {
		Assert.True(b);
	}

	public static void ShouldBeTrue(this bool b, string msg) {
		Assert.True(b, msg);
	}

	public static void ShouldBeFalse(this bool b) {
		Assert.False(b);
	}
}
