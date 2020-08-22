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

namespace FluentValidation.Tests {
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Xunit;

	public class InheritanceValidatorTest {

		[Fact]
		public void Validates_inheritance_hierarchy() {
			var validator = new InlineValidator<Root>();
			var impl1Validator = new InlineValidator<FooImpl1>();
			var impl2Validator = new InlineValidator<FooImpl2>();

			impl1Validator.RuleFor(x => x.Name).NotNull();
			impl2Validator.RuleFor(x => x.Number).GreaterThan(0);

			validator.RuleFor(x => x.Foo).SetInheritanceValidator(v => {
				v.Add(impl1Validator)
				 .Add(impl2Validator);
			});

			var result = validator.Validate(new Root {Foo = new FooImpl1()});
			result.Errors.Count.ShouldEqual(1);
			result.Errors[0].PropertyName.ShouldEqual("Foo.Name");

			result = validator.Validate(new Root {Foo = new FooImpl2()});
			result.Errors.Count.ShouldEqual(1);
			result.Errors[0].PropertyName.ShouldEqual("Foo.Number");
		}


		[Fact]
		public async Task Validates_inheritance_async() {
			var validator = new InlineValidator<Root>();
			var impl1Validator = new InlineValidator<FooImpl1>();
			var impl2Validator = new InlineValidator<FooImpl2>();

			impl1Validator.RuleFor(x => x.Name).MustAsync((s, token) => Task.FromResult(s != null));
			impl2Validator.RuleFor(x => x.Number).MustAsync((i, token) => Task.FromResult(i > 0));

			validator.RuleFor(x => x.Foo).SetInheritanceValidator(v => {
				v.Add(impl1Validator)
					.Add(impl2Validator);
			});

			var result = await validator.ValidateAsync(new Root {Foo = new FooImpl1()});
			result.Errors.Count.ShouldEqual(1);
			result.Errors[0].PropertyName.ShouldEqual("Foo.Name");

			result = await validator.ValidateAsync(new Root {Foo = new FooImpl2()});
			result.Errors.Count.ShouldEqual(1);
			result.Errors[0].PropertyName.ShouldEqual("Foo.Number");

		}

		[Fact]
		public void Validates_collection() {
			var validator = new InlineValidator<Root>();
			var impl1Validator = new InlineValidator<FooImpl1>();
			var impl2Validator = new InlineValidator<FooImpl2>();

			impl1Validator.RuleFor(x => x.Name).NotNull();
			impl2Validator.RuleFor(x => x.Number).GreaterThan(0);

			validator.RuleForEach(x => x.Foos).SetInheritanceValidator(v => {
				v.Add(impl1Validator)
					.Add(impl2Validator);
			});

			var result = validator.Validate(new Root {Foos = { new FooImpl1() }});
			result.Errors.Count.ShouldEqual(1);
			result.Errors[0].PropertyName.ShouldEqual("Foos[0].Name");

			result = validator.Validate(new Root {Foos = { new FooImpl2() }});
			result.Errors.Count.ShouldEqual(1);
			result.Errors[0].PropertyName.ShouldEqual("Foos[0].Number");
		}

		[Fact]
		public async Task Validates_collection_async() {
			var validator = new InlineValidator<Root>();
			var impl1Validator = new InlineValidator<FooImpl1>();
			var impl2Validator = new InlineValidator<FooImpl2>();

			impl1Validator.RuleFor(x => x.Name).MustAsync((s, token) => Task.FromResult(s != null));
			impl2Validator.RuleFor(x => x.Number).MustAsync((i, token) => Task.FromResult(i > 0));

			validator.RuleForEach(x => x.Foos).SetInheritanceValidator(v => {
				v.Add(impl1Validator)
					.Add(impl2Validator);
			});

			var result = await validator.ValidateAsync(new Root { Foos = { new FooImpl1() }});
			result.Errors.Count.ShouldEqual(1);
			result.Errors[0].PropertyName.ShouldEqual("Foos[0].Name");

			result = await validator.ValidateAsync(new Root { Foos = { new FooImpl2() }});
			result.Errors.Count.ShouldEqual(1);
			result.Errors[0].PropertyName.ShouldEqual("Foos[0].Number");
		}

		[Fact]
		public void Validates_with_callback() {
			var validator = new InlineValidator<Root>();
			var impl1Validator = new InlineValidator<FooImpl1>();
			var impl2Validator = new InlineValidator<FooImpl2>();

			impl1Validator.RuleFor(x => x.Name).NotNull();
			impl2Validator.RuleFor(x => x.Number).GreaterThan(0);

			validator.RuleFor(x => x.Foo).SetInheritanceValidator(v => {
				v.Add(x => impl1Validator)
					.Add(x => impl2Validator);
			});

			var result = validator.Validate(new Root {Foo = new FooImpl1()});
			result.Errors.Count.ShouldEqual(1);
			result.Errors[0].PropertyName.ShouldEqual("Foo.Name");

			result = validator.Validate(new Root {Foo = new FooImpl2()});
			result.Errors.Count.ShouldEqual(1);
			result.Errors[0].PropertyName.ShouldEqual("Foo.Number");

		}

		[Fact]
		public async Task Validates_with_callback_async() {
			var validator = new InlineValidator<Root>();
			var impl1Validator = new InlineValidator<FooImpl1>();
			var impl2Validator = new InlineValidator<FooImpl2>();

			impl1Validator.RuleFor(x => x.Name).MustAsync((s, token) => Task.FromResult(s != null));
			impl2Validator.RuleFor(x => x.Number).MustAsync((i, token) => Task.FromResult(i > 0));

			validator.RuleFor(x => x.Foo).SetInheritanceValidator(v => {
				v.Add(x => impl1Validator)
					.Add(x => impl2Validator);
			});

			var result = await validator.ValidateAsync(new Root {Foo = new FooImpl1()});
			result.Errors.Count.ShouldEqual(1);
			result.Errors[0].PropertyName.ShouldEqual("Foo.Name");

			result = await validator.ValidateAsync(new Root {Foo = new FooImpl2()});
			result.Errors.Count.ShouldEqual(1);
			result.Errors[0].PropertyName.ShouldEqual("Foo.Number");

		}

		[Fact]
		public void Validates_ruleset() {
			var validator = new InlineValidator<Root>();
			var impl1Validator = new InlineValidator<FooImpl1>();
			var impl2Validator = new InlineValidator<FooImpl2>();

			impl1Validator.RuleFor(x => x.Name).Equal("Foo");
			impl1Validator.RuleSet("RuleSet1", () => {
				impl1Validator.RuleFor(x => x.Name).NotNull();
			});


			impl2Validator.RuleFor(x => x.Number).Equal(42);
			impl2Validator.RuleSet("RuleSet2", () => {
				impl2Validator.RuleFor(x => x.Number).GreaterThan(0);
			});

			validator.RuleFor(x => x.Foo).SetInheritanceValidator(v => {
				v.Add(impl1Validator, "RuleSet1")
					.Add(impl2Validator, "RuleSet2");
			});

			var result = validator.Validate(new Root {Foo = new FooImpl1()});
			result.Errors.Count.ShouldEqual(1);
			result.Errors[0].PropertyName.ShouldEqual("Foo.Name");

			result = validator.Validate(new Root {Foo = new FooImpl2()});
			result.Errors.Count.ShouldEqual(1);
			result.Errors[0].PropertyName.ShouldEqual("Foo.Number");

		}

		[Fact]
		public async Task Validates_ruleset_async() {
			var validator = new InlineValidator<Root>();
			var impl1Validator = new InlineValidator<FooImpl1>();
			var impl2Validator = new InlineValidator<FooImpl2>();

			impl1Validator.RuleFor(x => x.Name).Equal("Foo");
			impl1Validator.RuleSet("RuleSet1", () => {
				impl1Validator.RuleFor(x => x.Name).MustAsync((s, token) => Task.FromResult(s != null));
			});


			impl2Validator.RuleFor(x => x.Number).Equal(42);
			impl2Validator.RuleSet("RuleSet2", () => {
				impl2Validator.RuleFor(x => x.Number).MustAsync((i, token) => Task.FromResult(i > 0));
			});

			validator.RuleFor(x => x.Foo).SetInheritanceValidator(v => {
				v.Add(impl1Validator, "RuleSet1")
					.Add(impl2Validator, "RuleSet2");
			});

			var result = await validator.ValidateAsync(new Root {Foo = new FooImpl1()});
			result.Errors.Count.ShouldEqual(1);
			result.Errors[0].PropertyName.ShouldEqual("Foo.Name");

			result = await validator.ValidateAsync(new Root {Foo = new FooImpl2()});
			result.Errors.Count.ShouldEqual(1);
			result.Errors[0].PropertyName.ShouldEqual("Foo.Number");

		}

		public interface IFoo {

		}

		public class FooImpl1 : IFoo {
			public string Name { get; set; }
		}

		public class FooImpl2 : IFoo {
			public int Number { get; set; }
		}

		public class Root {
			public IFoo Foo { get; set; }

			public List<IFoo> Foos { get; set; } = new List<IFoo>();
		}
	}
}
