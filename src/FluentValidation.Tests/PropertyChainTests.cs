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
	using System.Linq.Expressions;
	using Internal;
	using Xunit;
	using System.Reflection;

	
	public class PropertyChainTests {
		PropertyChain chain;

		public PropertyChainTests() {
			chain = new PropertyChain();
		}

		[Fact]
		public void Calling_ToString_should_construct_string_representation_of_chain() {

			chain.Add(typeof(Parent).GetProperty("Child"));
			chain.Add(typeof(Child).GetProperty("GrandChild"));
			const string expected = "Child.GrandChild";

			chain.ToString().ShouldEqual(expected);
		}

		[Fact]
		public void Calling_ToString_should_construct_string_representation_of_chain_with_indexers() {
			chain.Add(typeof(Parent).GetProperty("Child"));
			chain.AddIndexer(0);
			chain.Add(typeof(Child).GetProperty("GrandChild"));
			const string expected = "Child[0].GrandChild";

			chain.ToString().ShouldEqual(expected);
		}

		[Fact]
		public void AddIndexer_throws_when_nothing_added() {
			typeof(InvalidOperationException).ShouldBeThrownBy(() => chain.AddIndexer(0));
		}

		[Fact]
		public void Should_be_subchain() {
			chain.Add("Parent");
			chain.Add("Child");

			var childChain = new PropertyChain(chain);
			childChain.Add("Grandchild");

			childChain.IsChildChainOf(chain).ShouldBeTrue();
		}

		[Fact]
		public void Should_not_be_subchain() {
			chain.Add("Foo");

			var otherChain = new PropertyChain();
			otherChain.Add("Bar");

			otherChain.IsChildChainOf(chain).ShouldBeFalse();
		}

		[Fact]
		public void Creates_from_expression() {
			Expression<Func<Person, int>> expr = x => x.Address.Id;
			var chain = PropertyChain.FromExpression(expr);
			chain.ToString().ShouldEqual("Address.Id");
		}

		[Fact]
		public void Should_ignore_blanks() {
			chain.Add("");
			chain.Add("Foo");

			chain.ToString().ShouldEqual("Foo");
		}

		public class Parent {
			public Child Child { get; set; }
		}

		public class Child {
			public Grandchild GrandChild { get; set; }
		}

		public class Grandchild {}
	}
}