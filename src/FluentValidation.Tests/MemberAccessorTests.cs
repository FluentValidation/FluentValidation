namespace FluentValidation.Tests
{
	using System;
	using System.Linq.Expressions;
	using NUnit.Framework;

	[TestFixture]
	public class MemberAccessorTests {
		Person person;

		MemberAccessor<Person, string> nameFieldAccessor;
		MemberAccessor<Person, string> forenameAccessor;
		MemberAccessor<Person, string> countryNameAccessor;

		[SetUp]
		public void Setup() {
			person = new Person();
			person.NameField = "John Smith";
			person.Forename = "John";
			person.Address = new Address {Country = new Country {Name = "United States"}};

			nameFieldAccessor = MemberAccessor<Person>.From(x => x.NameField);
			forenameAccessor = MemberAccessor<Person>.From(x => x.Forename);
			countryNameAccessor = MemberAccessor<Person>.From(x => x.Address.Country.Name);
		}

		[Test]
		public void SimpleFieldGet() {
			Assert.AreEqual("John Smith", nameFieldAccessor.Get(person));
		}

		[Test]
		public void SimplePropertyGet() {
			Assert.AreEqual("John", forenameAccessor.Get(person));
		}

		[Test]
		public void SimpleFieldSet() {
			nameFieldAccessor.Set(person, "Peter Smith");
			Assert.AreEqual("Peter Smith", person.NameField);
		}

		[Test]
		public void SimplePropertySet() {
			forenameAccessor.Set(person, "Peter");
			Assert.AreEqual("Peter", person.Forename);
		}

		[Test]
		public void ComplexPropertyGet() {
			Assert.AreEqual("United States", countryNameAccessor.Get(person));
		}

		[Test]
		public void ComplexPropertySet() {
			countryNameAccessor.Set(person, "United Kingdom");
			Assert.AreEqual("United Kingdom", person.Address.Country.Name);
		}

		[Test]
		public void Equality() {
			Assert.AreEqual(nameFieldAccessor, MemberAccessor<Person>.From(x => x.NameField));
			Assert.AreNotEqual(nameFieldAccessor, forenameAccessor);
			Assert.AreNotEqual(nameFieldAccessor, countryNameAccessor);

			Assert.AreEqual(forenameAccessor, MemberAccessor<Person>.From(x => x.Forename));
			Assert.AreNotEqual(forenameAccessor, nameFieldAccessor);
			Assert.AreNotEqual(forenameAccessor, countryNameAccessor);

			Assert.AreEqual(countryNameAccessor, MemberAccessor<Person>.From(x => x.Address.Country.Name));
			Assert.AreNotEqual(countryNameAccessor, nameFieldAccessor);
			Assert.AreNotEqual(countryNameAccessor, forenameAccessor);
		}

		[Test]
		public void ImplicitCast() {
			Expression<Func<Person, string>> nameFieldAsExpression = nameFieldAccessor;
			MemberAccessor<Person, string> nameFieldAccessor2 = nameFieldAsExpression;
			Assert.AreEqual(nameFieldAccessor, nameFieldAccessor2);
		}

		[Test]
		public void Name() {
			Assert.AreEqual("NameField", nameFieldAccessor.Member.Name);
			Assert.AreEqual("Forename", forenameAccessor.Member.Name);
			Assert.AreEqual("Name", countryNameAccessor.Member.Name);

		}
	}
}
