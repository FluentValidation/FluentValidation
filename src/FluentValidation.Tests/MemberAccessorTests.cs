namespace FluentValidation.Tests
{
	using System;
	using System.Linq.Expressions;
	using Internal;
	using Xunit;

	
	public class MemberAccessorTests {
		Person person;

		MemberAccessor<Person, string> nameFieldAccessor;
		MemberAccessor<Person, string> forenameAccessor;
		MemberAccessor<Person, string> countryNameAccessor;

		public MemberAccessorTests() {
			person = new Person();
			person.NameField = "John Smith";
			person.Forename = "John";
			person.Address = new Address {Country = new Country {Name = "United States"}};

			nameFieldAccessor = new MemberAccessor<Person, string>(x => x.NameField, true);
			forenameAccessor = new MemberAccessor<Person, string>(x => x.Forename, true);
			countryNameAccessor = new MemberAccessor<Person, string>(x => x.Address.Country.Name, true);
		}

		[Fact]
		public void SimpleFieldGet() {
			Assert.Equal("John Smith", nameFieldAccessor.Get(person));
		}

		[Fact]
		public void SimplePropertyGet() {
			Assert.Equal("John", forenameAccessor.Get(person));
		}

		[Fact]
		public void SimpleFieldSet() {
			nameFieldAccessor.Set(person, "Peter Smith");
			Assert.Equal("Peter Smith", person.NameField);
		}

		[Fact]
		public void SimplePropertySet() {
			forenameAccessor.Set(person, "Peter");
			Assert.Equal("Peter", person.Forename);
		}

		[Fact]
		public void ComplexPropertyGet() {
			Assert.Equal("United States", countryNameAccessor.Get(person));
		}

		[Fact]
		public void ComplexPropertySet() {
			countryNameAccessor.Set(person, "United Kingdom");
			Assert.Equal("United Kingdom", person.Address.Country.Name);
		}

		[Fact]
		public void Equality() {
			Assert.Equal(nameFieldAccessor, new MemberAccessor<Person, string>(x => x.NameField, true));
			Assert.NotEqual(nameFieldAccessor, forenameAccessor);
			Assert.NotEqual(nameFieldAccessor, countryNameAccessor);

			Assert.Equal(forenameAccessor, new MemberAccessor<Person, string>(x => x.Forename, true));
			Assert.NotEqual(forenameAccessor, nameFieldAccessor);
			Assert.NotEqual(forenameAccessor, countryNameAccessor);

			Assert.Equal(countryNameAccessor, new MemberAccessor<Person, string>(x => x.Address.Country.Name, true));
			Assert.NotEqual(countryNameAccessor, nameFieldAccessor);
			Assert.NotEqual(countryNameAccessor, forenameAccessor);
		}

		[Fact]
		public void ImplicitCast() {
			Expression<Func<Person, string>> nameFieldAsExpression = nameFieldAccessor;
			MemberAccessor<Person, string> nameFieldAccessor2 = nameFieldAsExpression;
			Assert.Equal(nameFieldAccessor, nameFieldAccessor2);
		}

		[Fact]
		public void Name() {
			Assert.Equal("NameField", nameFieldAccessor.Member.Name);
			Assert.Equal("Forename", forenameAccessor.Member.Name);
			Assert.Equal("Name", countryNameAccessor.Member.Name);

		}
	}
}
