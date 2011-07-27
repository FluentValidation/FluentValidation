namespace FluentValidation.Tests
{
	using NUnit.Framework;
	using System;

	[TestFixture]
	public class SharedConditionTests
	{
		private class SharedConditionValidator : AbstractValidator<Person>
		{
			public SharedConditionValidator()
			{
				// Start with a predicate to group rules together.
				// 
				// The AbstractValidator appends this predicate
				// to each inner RuleFor so you only need write,
				// maintain, and think about it in one place.
				//
				// Each grouped RuleFor call must be wrapped in a 
				// chained Add() calls in order to enable chaining
				// RuleFor's that validate properties of multiple
				// types.
				// 
				// You can finish with an Unless clause that will
				// void the validation for the entire set when it's 
				// predicate is true.
				// 
				When( x => x.Id > 0 )
					.Add( RuleFor( x => x.Forename ).NotEmpty() )
					.Add( RuleFor( x => x.Surname ).NotEmpty().Equal( "Smith" ) );
			}
		}

		private class SharedConditionWithScopedUnlessValidator : AbstractValidator<Person>
		{
			public SharedConditionWithScopedUnlessValidator()
			{
				// inner RuleFor() calls can contain their own,
				// locally scoped When and Unless calls that
				// act only on that individual RuleFor() yet the
				// RuleFor() respects the grouped When() and 
				// Unless() predicates.
				// 
				When( x => x.Id > 0 )
					.Add( RuleFor( x => x.Orders.Count ).Equal( 0 ).Unless( x => String.IsNullOrWhiteSpace( x.CreditCard ) == false ) )
					.Unless( x => x.Age > 65 );
			}
		}

		[Test]
		public void Shared_When_is_not_applied_to_grouped_rules_when_initial_predicate_is_false()
		{
			var validator = new SharedConditionValidator();
			var person = new Person();	// fails the shared When predicate

			var result = validator.Validate( person );
			result.Errors.Count.ShouldEqual( 0 );
		}

		[Test]
		public void Shared_When_is_applied_to_grouped_rules_when_initial_predicate_is_true()
		{
			var validator = new SharedConditionValidator();
			var person = new Person()
			{
				Id = 4					// triggers the shared When predicate
			};

			var result = validator.Validate( person );
			result.Errors.Count.ShouldEqual( 3 );
		}

		[Test]
		public void Shared_When_is_applied_to_groupd_rules_when_initial_predicate_is_true_and_all_individual_rules_are_satisfied()
		{
			var validator = new SharedConditionValidator();
			var person = new Person()
			{
				Id       = 4,			// triggers the shared When predicate
				Forename = "Kevin",		// satisfies RuleFor( x => x.Forename ).NotEmpty()
				Surname  = "Smith",		// satisfies RuleFor( x => x.Surname ).NotEmpty().Equal( "Smith" )
			};

			var result = validator.Validate( person );
			result.Errors.Count.ShouldEqual( 0 );
		}

		[Test]
		public void Shared_When_respects_the_smaller_scope_of_an_inner_Unless_when_the_inner_Unless_predicate_is_satisfied()
		{
			var validator = new SharedConditionWithScopedUnlessValidator();
			var person = new Person()
			{
				Id       = 4						// triggers the shared When predicate
			};

			person.CreditCard = "1234123412341234"; // satisfies the inner Unless predicate
			person.Orders.Add( new Order() );

			var result = validator.Validate( person );
			result.Errors.Count.ShouldEqual( 0 );
		}

		[Test]
		public void Shared_When_respects_the_smaller_scope_of_a_inner_Unless_when_the_inner_Unless_predicate_fails()
		{
			var validator = new SharedConditionWithScopedUnlessValidator();
			var person = new Person()
			{
				Id  =  4							// triggers the shared When predicate
			};

			person.Orders.Add( new Order() );		// fails the inner Unless predicate

			var result = validator.Validate( person );
			result.Errors.Count.ShouldEqual( 1 );
		}

		[Test]
		public void Outer_Until_clause_will_trump_an_inner_Until_clause_when_inner_fails_but_the_outer_is_satisfied()
		{
			var validator = new SharedConditionWithScopedUnlessValidator();
			var person = new Person()
			{
				Id  =  4,							// triggers the shared When predicate
				Age	= 70							// satisfies the outer Unless predicate
			};

			person.Orders.Add( new Order() );		// fails the inner Unless predicate

			var result = validator.Validate( person );
			result.Errors.Count.ShouldEqual( 0 );
		}
	}
}
