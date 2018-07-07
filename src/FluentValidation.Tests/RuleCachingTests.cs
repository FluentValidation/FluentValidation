namespace FluentValidation.Tests {
	using System;
	using System.Linq;
	using System.Runtime.CompilerServices;
	using Internal;
	using Xunit;

	public class RuleCachingTests {

		public RuleCachingTests() {
			RuleCache.Clear();
		}
		
		[Fact]
		public void Caches_rules() {
			var validator = new CacheTestValidator();
			var rule1 = validator.First();
			
			var validator2 = new CacheTestValidator();
			var rule2 = validator2.First();

			Assert.Same(rule1, rule2);
		}

		[Fact]
		public void Rules_added_after_construction_only_apply_to_that_instance() {
			var validator = new CacheTestValidator();
			validator.Count().ShouldEqual(1);
			
			var validator2 = new CacheTestValidator();
			validator2.Count().ShouldEqual(1);
			
			validator2.RuleFor(x => x.Forename).NotNull();

			validator.Count().ShouldEqual(1);
			validator2.Count().ShouldEqual(2);
		}

		[Fact]
		public void Old_AbstractValidator_doesnt_cache() {
			var validator1 = new NonCachingValidator();
			var validator2 = new NonCachingValidator();

			validator1.Count().ShouldEqual(1);
			validator2.Count().ShouldEqual(1);
			
			Assert.NotSame(validator1.First(), validator2.First());

			RuleCache.TryGetRules(typeof(NonCachingValidator), out _).ShouldBeFalse();
			
		}

		
		[Fact]
		public void InlineValidator_doesnt_cache() {
			var validator1 = new InlineValidator<Person>();
			var validator2 = new InlineValidator<Person>();

			validator1.RuleFor(x => x.Surname).NotNull();
			validator1.Count().ShouldEqual(1);

			validator2.RuleFor(x => x.Surname).NotNull();
			validator2.Count().ShouldEqual(1);
			
			Assert.NotSame(validator1.First(), validator2.First());
			RuleCache.TryGetRules(typeof(NonCachingValidator), out _).ShouldBeFalse();
			
		}

		[Fact]
		public void Throws_if_user_tries_to_define_rules_in_validatorbase_constructor() {
			typeof(InvalidOperationException).ShouldBeThrownBy(() => {
				var v = new BadValidator();
				v.Validate(new Person());
			});
		}

		private class CacheTestValidator : ValidatorBase<Person> {
			protected override void Rules() {
				RuleFor(x => x.Surname).NotNull();
			}
		}

		private class NonCachingValidator : AbstractValidator<Person> {
			public NonCachingValidator() {
				RuleFor(x => x.Surname).NotNull();
			}
		}

		private class BadValidator : ValidatorBase<Person> {
			public BadValidator() {
				RuleFor(x => x.Surname).NotNull();
			}

			protected override void Rules() {
				
			}
		}
	}
}