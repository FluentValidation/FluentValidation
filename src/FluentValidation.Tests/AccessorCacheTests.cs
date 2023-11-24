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

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq.Expressions;
using Internal;
using Xunit;
using Xunit.Abstractions;

public class AccessorCacheTests {
	private readonly ITestOutputHelper output;

	public AccessorCacheTests(ITestOutputHelper output) {
		this.output = output;
		AccessorCache<Person>.Clear();
	}

	[Fact]
	public void Gets_accessor() {
		Expression<Func<Person, int>> expr1 = x => 1;

		var compiled1 = expr1.Compile();
		var compiled2 = expr1.Compile();

		Assert.NotEqual(compiled1, compiled2);

		var compiled3 = AccessorCache<Person>.GetCachedAccessor(typeof(Person).GetProperty("Id"), expr1);
		var compiled4 = AccessorCache<Person>.GetCachedAccessor(typeof(Person).GetProperty("Id"), expr1);

		Assert.Equal(compiled3, compiled4);
	}

	[Fact]
	public void Gets_accessor_for_parameter_expression() {
		Expression<Func<Person, Person>> expr1 = x => x;

		var compiled1 = expr1.Compile();
		var compiled2 = expr1.Compile();

		Assert.NotEqual(compiled1, compiled2);

		var compiled3 = AccessorCache<Person>.GetCachedAccessor(null, expr1);
		var compiled4 = AccessorCache<Person>.GetCachedAccessor(null, expr1);

		// Expression should have been cached.
		Assert.Equal(compiled3, compiled4);

		Expression<Func<Person, Person>> expr2 = x => x;
		var compiled5 = AccessorCache<Person>.GetCachedAccessor(null, expr2);

		// Technically a different expression, but for the same type, so should still be cached.
		Assert.Equal(compiled3, compiled5);

		// Different expression for a different type. Shouldn't try and cache and cast.
		Expression<Func<Address, Address>> expr3 = x => x;
		var compiled6 = AccessorCache<Address>.GetCachedAccessor(null, expr3);
	}

	[Fact]
	public void Equality_comparison_check() {
		Expression<Func<Person, string>> expr1 = x => x.Surname;
		Expression<Func<Person, string>> expr2 = x => x.Surname;
		Expression<Func<Person, string>> expr3 = x => x.Forename;

		var member1 = expr1.GetMember();
		var member2 = expr2.GetMember();
		var member3 = expr3.GetMember();

		Assert.Equal(member1, member2);
		Assert.NotEqual(member1, member3);
	}

	[Fact]
	public void Identifies_if_memberexp_acts_on_model_instance() {
		Expression<Func<Person, string>> expr1 = x => DoStuffToPerson(x).Surname;
		Expression<Func<Person, string>> expr2 = x => x.Surname;

		expr1.GetMember().ShouldBeNull();
		expr2.GetMember().ShouldNotBeNull();
	}

	[Fact]
	public void Gets_member_for_nested_property() {
		Expression<Func<Person, string>> expr1 = x => x.Address.Line1;
		expr1.GetMember().ShouldNotBeNull();
	}

	[Fact]
	public void No_error_when_accessing_same_property_via_different_collection_type_when_using_different_cache_prefix() {
		Expression<Func<Person, IEnumerable<Order>>> expr1 = x => x.Orders;
		Expression<Func<Person, IList<Order>>> expr2 = x => x.Orders;

		var accessor1 = AccessorCache<Person>.GetCachedAccessor(typeof(Person).GetProperty("Orders"), expr1, false, "Prefix1");
		var accessor2 = AccessorCache<Person>.GetCachedAccessor(typeof(Person).GetProperty("Orders"), expr2, false, "Prefix2");
		Assert.NotEqual(accessor1, accessor2);
	}

	private Person DoStuffToPerson(Person p) {
		return p;
	}

	[Fact(Skip = "Manual benchmark")]
	public void Benchmark() {
		var s = new Stopwatch();
		s.Start();

		for (int i = 0; i < 20000; i++) {
			var v = new BenchmarkValidator();
		}

		s.Stop();
		output.WriteLine(s.Elapsed.ToString());
	}

	private class BenchmarkValidator : AbstractValidator<Person> {
		public BenchmarkValidator() {
			RuleFor(x => x.Surname).NotNull();
			RuleFor(x => x).Must(x => true);
		}
	}

	private class CacheTestModel {
		[Display(Name = "Foo")]
		public string Name { get; set; }
	}
}
