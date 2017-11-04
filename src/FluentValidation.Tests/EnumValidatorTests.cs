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
	using System.Linq;
	using Xunit;

	public class EnumValidatorTests
	{
		TestValidator validator;

		public EnumValidatorTests()
		{
			CultureScope.SetDefaultCulture();

			validator = new TestValidator {
				v => v.RuleFor(x => x.Gender).IsInEnum()
			};
		}

		[Fact]
		public void IsValidTests()
		{
			validator.Validate(new Person { Gender = EnumGender.Female }).IsValid.ShouldBeTrue();  // Simplest valid value
			validator.Validate(new Person { Gender = EnumGender.Male }).IsValid.ShouldBeTrue();    // Other valid value
			validator.Validate(new Person { Gender = (EnumGender)1 }).IsValid.ShouldBeTrue();      // Casting with valid value
		}

		[Fact]
		public void When_the_enum_is_not_initialized_with_valid_value_then_the_validator_should_fail()
		{
			var result = validator.Validate(new Person());                                         // Default value 0 is not defined in Enum
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void When_the_enum_is_initialized_with_invalid_value_then_the_validator_should_fail()
		{
			var result = validator.Validate(new Person { Gender = (EnumGender)3 });                // 3 in not defined in Enum
			result.IsValid.ShouldBeFalse();
		}


		[Fact]
		public void When_validation_fails_the_default_error_should_be_set()
		{
			var result = validator.Validate(new Person());
			result.Errors.Single().ErrorMessage.ShouldEqual("'Gender' has a range of values which does not include '0'.");
		}

		[Fact]
		public void Nullable_enum_valid_when_property_value_is_null()
		{
			var validator = new InlineValidator<Foo>();
			validator.RuleFor(x => x.Gender).IsInEnum();
			var result = validator.Validate(new Foo());
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void Nullable_enum_valid_when_value_specified()
		{
			var validator = new InlineValidator<Foo>();
			validator.RuleFor(x => x.Gender).IsInEnum();
			var result = validator.Validate(new Foo() { Gender = EnumGender.Male });
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void Nullable_enum_invalid_when_bad_value_specified()
		{
			var validator = new InlineValidator<Foo>();
			validator.RuleFor(x => x.Gender).IsInEnum();
			var result = validator.Validate(new Foo() { Gender = (EnumGender)42 });
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void Flags_enum_valid_when_using_bitwise_value()
		{
			var inlineValidator = Create_validator();
			var poco = new FlagsEnumPoco();
			poco.PopulateWithValidValues();

			var result = inlineValidator.Validate(poco);
			result.IsValid.ShouldBeTrue();

			// special case - valid negative value
			poco.EnumWithNegativesValue = EnumWithNegatives.All;
			result = inlineValidator.Validate(poco);
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void Flags_enum_with_overlapping_flags_valid_when_using_bitwise_value()
		{
			var inlineValidator = new InlineValidator<FlagsEnumPoco>();
			inlineValidator.RuleFor(x => x.EnumWithOverlappingFlagsValue).IsInEnum();

			var poco = new FlagsEnumPoco();
			
			// test all combinations
			poco.EnumWithOverlappingFlagsValue = EnumWithOverlappingFlags.A | EnumWithOverlappingFlags.B;
			inlineValidator.Validate(poco).IsValid.ShouldBeTrue();

			poco.EnumWithOverlappingFlagsValue = EnumWithOverlappingFlags.B | EnumWithOverlappingFlags.C;
			inlineValidator.Validate(poco).IsValid.ShouldBeTrue();

			poco.EnumWithOverlappingFlagsValue = EnumWithOverlappingFlags.A | EnumWithOverlappingFlags.C;
			inlineValidator.Validate(poco).IsValid.ShouldBeTrue();

			poco.EnumWithOverlappingFlagsValue = EnumWithOverlappingFlags.A | EnumWithOverlappingFlags.B | EnumWithOverlappingFlags.C;
			inlineValidator.Validate(poco).IsValid.ShouldBeTrue();
		}

		[Fact]
		public void Flags_enum_validates_correctly_when_using_zero_value()
		{
			var inlineValidator = Create_validator();

			var poco = new FlagsEnumPoco();

			// all default to zero
			var result = inlineValidator.Validate(poco);

			result.Errors.SingleOrDefault(x => x.PropertyName == "EnumWithNegativesValue").ShouldNotBeNull();
			result.Errors.SingleOrDefault(x => x.PropertyName == "EnumWithOverlappingFlagsValue").ShouldNotBeNull();
			result.Errors.Count().ShouldEqual(2);
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void Flags_enum_invalid_when_using_outofrange_positive_value()
		{
			var inlineValidator = Create_validator();

			var poco = new FlagsEnumPoco();
			poco.PopulateWithInvalidPositiveValues();

			var result = inlineValidator.Validate(poco);
			result.IsValid.ShouldBeFalse();
			result.Errors.SingleOrDefault(x => x.PropertyName == "ByteValue").ShouldNotBeNull();
			result.Errors.SingleOrDefault(x => x.PropertyName == "SByteValue").ShouldNotBeNull();
			result.Errors.SingleOrDefault(x => x.PropertyName == "Int16Value").ShouldNotBeNull();
			result.Errors.SingleOrDefault(x => x.PropertyName == "UInt16Value").ShouldNotBeNull();
			result.Errors.SingleOrDefault(x => x.PropertyName == "Int32Value").ShouldNotBeNull();
			result.Errors.SingleOrDefault(x => x.PropertyName == "UInt32Value").ShouldNotBeNull();
			result.Errors.SingleOrDefault(x => x.PropertyName == "Int64Value").ShouldNotBeNull();
			result.Errors.SingleOrDefault(x => x.PropertyName == "UInt64Value").ShouldNotBeNull();			
			result.Errors.SingleOrDefault(x => x.PropertyName == "EnumWithNegativesValue").ShouldNotBeNull();
		}

		[Fact]
		public void Flags_enum_invalid_when_using_outofrange_negative_value()
		{
			var inlineValidator = Create_validator();

			var poco = new FlagsEnumPoco();
			poco.PopulateWithInvalidNegativeValues();

			var result = inlineValidator.Validate(poco);
			result.IsValid.ShouldBeFalse();
			result.Errors.SingleOrDefault(x => x.PropertyName == "SByteValue").ShouldNotBeNull();
			result.Errors.SingleOrDefault(x => x.PropertyName == "Int16Value").ShouldNotBeNull();
			result.Errors.SingleOrDefault(x => x.PropertyName == "Int32Value").ShouldNotBeNull();
			result.Errors.SingleOrDefault(x => x.PropertyName == "Int64Value").ShouldNotBeNull();
		}

		private InlineValidator<FlagsEnumPoco> Create_validator()
		{
			var inlineValidator = new InlineValidator<FlagsEnumPoco>();
			inlineValidator.RuleFor(x => x.SByteValue).IsInEnum();
			inlineValidator.RuleFor(x => x.ByteValue).IsInEnum();
			inlineValidator.RuleFor(x => x.Int16Value).IsInEnum();
			inlineValidator.RuleFor(x => x.UInt16Value).IsInEnum();
			inlineValidator.RuleFor(x => x.Int32Value).IsInEnum();
			inlineValidator.RuleFor(x => x.UInt32Value).IsInEnum();
			inlineValidator.RuleFor(x => x.Int64Value).IsInEnum();
			inlineValidator.RuleFor(x => x.UInt64Value).IsInEnum();
			inlineValidator.RuleFor(x => x.EnumWithNegativesValue).IsInEnum();
			inlineValidator.RuleFor(x => x.EnumWithOverlappingFlagsValue).IsInEnum();

			return inlineValidator;
		}

		private class Foo
		{
			public EnumGender? Gender { get; set; }
		}

		#region Flag enum helpers
		private class FlagsEnumPoco
		{
			public SByteEnum SByteValue { get; set; }
			public ByteEnum ByteValue { get; set; }
			public Int16Enum Int16Value { get; set; }
			public UInt16Enum UInt16Value { get; set; }
			public Int32Enum Int32Value { get; set; }
			public UInt32Enum UInt32Value { get; set; }
			public Int64Enum Int64Value { get; set; }
			public UInt64Enum UInt64Value { get; set; }
			public EnumWithNegatives EnumWithNegativesValue { get; set; }
			public EnumWithOverlappingFlags EnumWithOverlappingFlagsValue { get; set;}

			public void PopulateWithValidValues()
			{
				SByteValue = SByteEnum.B | SByteEnum.C;
				ByteValue = ByteEnum.B | ByteEnum.C;
				Int16Value = Int16Enum.B | Int16Enum.C;
				UInt16Value = UInt16Enum.B | UInt16Enum.C;
				Int32Value = Int32Enum.B | Int32Enum.C;
				UInt32Value = UInt32Enum.B | UInt32Enum.C;
				Int64Value = Int64Enum.B | Int64Enum.C;
				UInt64Value = UInt64Enum.B | UInt64Enum.C;
				EnumWithNegativesValue = EnumWithNegatives.Bar;
				EnumWithOverlappingFlagsValue = EnumWithOverlappingFlags.A;
			}

			public void PopulateWithInvalidPositiveValues()
			{
				SByteValue = (SByteEnum)123;
				ByteValue = (ByteEnum)123;
				Int16Value = (Int16Enum)123;
				UInt16Value = (UInt16Enum)123;
				Int32Value = (Int32Enum)123;
				UInt32Value = (UInt32Enum)123;
				Int64Value = (Int64Enum)123;
				UInt64Value = (UInt64Enum)123;
				EnumWithNegativesValue = (EnumWithNegatives)123;
				EnumWithOverlappingFlagsValue = (EnumWithOverlappingFlags)123;
			}

			public void PopulateWithInvalidNegativeValues()
			{
				SByteValue = (SByteEnum)(-123);
				Int16Value = (Int16Enum)(-123);
				Int32Value = (Int32Enum)(-123);
				Int64Value = (Int64Enum)(-123);
				EnumWithNegativesValue = (EnumWithNegatives)(-123);
				EnumWithOverlappingFlagsValue = (EnumWithOverlappingFlags)(-123);
			}
		}

		[Flags]
		private enum SByteEnum : sbyte
		{
			A = 0,
			B = 1,
			C = 2
		}

		[Flags]
		private enum ByteEnum : byte
		{
			A = 0,
			B = 1,
			C = 2
		}

		[Flags]
		private enum Int16Enum : short
		{
			A = 0,
			B = 1,
			C = 2
		}

		[Flags]
		private enum UInt16Enum : ushort
		{
			A = 0,
			B = 1,
			C = 2
		}

		[Flags]
		private enum Int32Enum : int
		{
			A = 0,
			B = 1,
			C = 2
		}

		[Flags]
		private enum UInt32Enum : uint
		{
			A = 0,
			B = 1,
			C = 2
		}

		[Flags]
		private enum Int64Enum : long
		{
			A = 0,
			B = 1,
			C = 2
		}

		[Flags]
		private enum UInt64Enum : ulong
		{
			A = 0,
			B = 1,
			C = 2
		}

		[Flags]
		public enum EnumWithNegatives
		{
			All = ~0,
			Bar = 1,
			Foo = 2
		}

		// NB this enum actually confuses the built-in Enum.ToString() functionality - it shows 7 for A|B.
		[Flags]
		public enum EnumWithOverlappingFlags
		{
			A = 3,
			B = 4,
			C = 5
		}
		#endregion
	}
}