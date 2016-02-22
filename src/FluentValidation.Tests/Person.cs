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

namespace FluentValidation.Tests
{
	using System;
	using System.Collections.Generic;
	using Attributes;

	[Validator(typeof(TestValidator))]
	public class Person {
		public string NameField;
		public int Id { get; set; }
		public string Surname { get; set; }
		public string Forename { get; set; }

		public List<Person> Children { get; set; }
		public string[] NickNames { get; set; }

		public DateTime DateOfBirth { get; set; }

		public int? NullableInt { get; set; }

		public Person() {
			Children = new List<Person>();
			Orders = new List<Order>();
		}

		public int CalculateSalary() {
			return 20;
		}

		public Address Address { get; set; }
		public IList<Order> Orders { get; set; }

		public string Email { get; set; }
		public decimal Discount { get; set; }
		public double Age { get; set; }

		public int AnotherInt { get; set; }

		public string CreditCard { get; set; }

		public int? OtherNullableInt { get; set; }

		public string Regex { get; set; }

		public System.Text.RegularExpressions.Regex AnotherRegex { get; set; }

		public int MinLength { get; set; }

		public int MaxLength { get; set; }

        public EnumGender Gender { get; set; } 
	}


	public interface IAddress {
		string Line1 { get; set; }
		string Line2 { get; set; }
		string Town { get; set; }
		string County { get; set; }
		string Postcode { get; set; }
		Country Country { get; set; }
	}

	public class Address : IAddress {
		public string Line1 { get; set; }
		public string Line2 { get; set; }
		public string Town { get; set; }
		public string County { get; set; }
		public string Postcode { get; set; }
		public Country Country { get; set; }
		public int Id { get; set; }
	}

	public class Country {
		public string Name { get; set; }
	}

	public interface IOrder {
		decimal Amount { get; }
	}

	public class Order : IOrder {
		public string ProductName { get; set; }
		public decimal Amount { get; set; }
	}

    public enum EnumGender
    {
        Female = 1,
        Male = 2
    }
}
