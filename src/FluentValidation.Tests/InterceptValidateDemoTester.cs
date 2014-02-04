using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentValidation.Tests
{
	using System.Linq.Expressions;
	using System.Reflection;
	using NUnit.Framework;

	[TestFixture]
	public class InterceptValidateDemoTester {

		[Test]
		public void TestInterception() {
			// we never want this object to be invalid 
			// (probably read from a database)
			var c = new InterceptClass();

			// the good setters
			c.Name = "Matthew";
			Assert.AreEqual("Matthew", c.Name);
			c.Age = 19;
			Assert.AreEqual(19, c.Age);

			try {
				// the baddies
				c.Name = null;
				Assert.Fail("Should not allow setting of invalid data.");

				// we would maybe do this:
				//		database.Save(c);
			} catch (ValidationException ex) {
				// don't allow the bad data
			}

			// we can safely do database.Save(c) as it will be valid
			// this allows for automated tasks to never save invalid data
			Assert.IsTrue(c.IsValid);
		}

		public class InterceptClass {
			private string name;
			private int age;

			public string Name
			{
				get { return name; }
				set { ValidateSet(ref name, x => x.Name, value); }
			}

			public int Age {
				get { return age; }
				set { ValidateSet(ref age, x => x.Age, value); }
			}

			public bool IsValid {
				get { return theValidator.Validate(this).IsValid; }
			}

			protected void ValidateSet<T>(ref T field, Expression<Func<InterceptClass, object>> property, T value) {
				var result = theValidator.ValidateMember(this, property, value);

				if (result.IsValid) {
					field = value;
				} else {
					throw new ValidationException(result.Errors);
				}
			}

			private readonly InlineValidator<InterceptClass> theValidator =
				new InlineValidator<InterceptClass> {
						v => v.RuleFor(x=> x.Name).NotNull().Length(1, 100),
						v => v.RuleFor(x=> x.Age).InclusiveBetween(1, 99)
					};
		}
	}
}
