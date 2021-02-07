﻿namespace FluentValidation.Tests {
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
}
