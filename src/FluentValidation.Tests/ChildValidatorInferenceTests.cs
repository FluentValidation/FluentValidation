namespace FluentValidation.Tests {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using Internal;
	using NUnit.Framework;
	using Validators;

	[TestFixture]
	public class ChildValidatorInferenceTests {

		[Test]
		public void Infers_validator_for_complex_property() {
			var validator = GetValidator(x => x.Person, new InlineValidator<Person>());
			validator.ShouldBe<ChildValidatorAdaptor>();
		}

		[Test]
		public void Infers_validator_for_ienumerable() {
			var validator = GetValidator(x => x.People, new InlineValidator<Person>());
			validator.ShouldBe<ChildCollectionValidatorAdaptor>();
		}

		[Test]
		public void Infers_validator_for_list() {
			var validator = GetValidator(x => x.PeopleList, new InlineValidator<Person>());
			validator.ShouldBe<ChildCollectionValidatorAdaptor>();
		}

		[Test]
		public void Infers_validator_for_collection() {
			var validator = GetValidator(x => x.PeopleCollection, new InlineValidator<Person>());
			validator.ShouldBe<ChildCollectionValidatorAdaptor>();
		}

		[Test]
		public void Infers_validator_for_string() {
			var validator = GetValidator(x => x.StringProperty, new InlineValidator<string>());
			validator.ShouldBe<ChildValidatorAdaptor>();
		}

		[Test]
		public void Infers_validator_for_ienumerable_char() {
			var validator = GetValidator(x => x.StringProperty, new InlineValidator<char>());
			validator.ShouldBe<ChildCollectionValidatorAdaptor>();
		}

		[Test]
		public void Infers_validator_for_custom_type_that_implements_ienumerable() {
			var validator = GetValidator(x => x.PeopleCustom, new InlineValidator<PersonCollection>());
			validator.ShouldBe<ChildValidatorAdaptor>();
		}

		[Test]
		public void Infers_collection_validator_for_custom_type_that_implements_ienumerable() {
			var validator = GetValidator(x => x.PeopleCustom, new InlineValidator<Person>());
			validator.ShouldBe<ChildCollectionValidatorAdaptor>();
		}

		[Test]
		public void Infers_validator_for_subclass() {
			var validator = GetValidator(x => x.DerivedPerson, new InlineValidator<Person>());
			validator.ShouldBe<ChildValidatorAdaptor>();
		}

		[Test]
		public void Throws_when_validator_of_wrong_type() {
			typeof(InvalidOperationException).ShouldBeThrownBy(() => GetValidator(x => x.StringProperty, new InlineValidator<Person>()));
		}

		private IPropertyValidator GetValidator<T>(Expression<Func<Demo, T >> expr, IValidator childValidator) {
			var validator = new InlineValidator<Demo>();
#pragma warning disable 612,618
			validator.RuleFor(expr).SetValidator(childValidator);
#pragma warning restore 612,618

			var rule = (PropertyRule)validator.Single();
			return rule.Validators.Single();
		}

		private class Demo {
			public Person Person { get; set; }
			public IEnumerable<Person> People { get; set; }
			public IList<Person> PeopleList { get; set; }
			public ICollection<Person> PeopleCollection { get; set; }
			public string StringProperty { get; set; }
			public PersonCollection PeopleCustom { get; set; }
			public DerivedPerson DerivedPerson { get; set; }
		}

		private class DerivedPerson : Person { }

		public class PersonCollection : List<Person> {
		}
	}

}