namespace FluentValidation.Tests {
	using System;
	using System.Linq;
	using Attributes;
	using Moq;
	using NUnit.Framework;
	using xVal.RuleProviders;
	using xVal.Rules;
	using xValIntegration;

	[TestFixture]
	public class xValRuleProviderTester {
		IRulesProvider provider;
		TestValidator validator;

		[SetUp]
		public void Setup() {
			validator = new TestValidator();
			var validatorFactory = new Mock<IValidatorFactory>();
			validatorFactory.Setup(x => x.GetValidator(typeof(Person))).Returns(validator);
			provider = new FluentValidationRulesProvider(validatorFactory.Object);
		}

		[Test]
		public void Converts_NotNullValidator_to_RequiredRule() {
			validator.RuleFor(x => x.Surname).NotNull();

			var rules = provider.GetRulesFromType(typeof(Person));
			rules["Surname"].Single().ShouldBe<RequiredRule>();
		}

		[Test]
		public void Converts_NotEmptyValidator_to_RequiredRule() {
			validator.RuleFor(x => x.Surname).NotEmpty();

			var rules = provider.GetRulesFromType(typeof(Person));
			rules["Surname"].Single().ShouldBe<RequiredRule>();
		}

		[Test]
		public void Converts_LengthValidator_to_StringLengthRule() {
			validator.RuleFor(x => x.Surname).Length(1, 20);

			var rules = provider.GetRulesFromType(typeof(Person));

			var rule = rules["Surname"].Single().ShouldBe<StringLengthRule>();
			rule.MaxLength.ShouldEqual(20);
			rule.MinLength.ShouldEqual(1);
		}

		[Test]
		public void Converts_RegularExpressionValidator_to_RegularExpressionRule() {
			validator.RuleFor(x => x.Surname).Matches("foo");

			var rules = provider.GetRulesFromType(typeof(Person));
			var rule = rules["Surname"].Single().ShouldBe<RegularExpressionRule>();
			rule.Pattern.ShouldEqual("foo");

		}

		[Test]
		public void Converts_EqualValidator_to_ComparisonRule() {
			validator.RuleFor(x => x.Surname).Equal(x => x.Forename);

			var rules = provider.GetRulesFromType(typeof(Person));
			var rule = rules["Surname"].Single().ShouldBe<ComparisonRule>();
			rule.ComparisonOperator.ShouldEqual(ComparisonRule.Operator.Equals);
			rule.PropertyToCompare.ShouldEqual("Forename");
		}

		[Test]
		public void Converts_NotEqualValidator_to_ComparisonRule() {
			validator.RuleFor(x => x.Surname).NotEqual(x => x.Forename);

			var rules = provider.GetRulesFromType(typeof(Person));
			var rule = rules["Surname"].Single().ShouldBe<ComparisonRule>();
			rule.ComparisonOperator.ShouldEqual(ComparisonRule.Operator.DoesNotEqual);
			rule.PropertyToCompare.ShouldEqual("Forename");
		}

		[Test]
		public void Converts_EmailValidator_to_DataTypeRule() {
			validator.RuleFor(x => x.Email).EmailAddress();

			var rules = provider.GetRulesFromType(typeof(Person));
			var rule = rules["Email"].Single().ShouldBe<DataTypeRule>();
			rule.Type.ShouldEqual(DataTypeRule.DataType.EmailAddress);
		}

		[Test]
		public void Converts_DelegatingValidator_to_underlying_validator() {
			validator.RuleFor(x => x.Surname).NotNull().When(x => true);

			var rules = provider.GetRulesFromType(typeof(Person));
			rules["Surname"].Single().ShouldBe<RequiredRule>();
		}


		[Test]
		public void Converts_LessThanOrEqual_to_RangeRule_for_int() {
			validator.RuleFor(x => x.Id).LessThanOrEqualTo(5);

			var rules = provider.GetRulesFromType(typeof(Person));
			var rule = rules["Id"].Single().ShouldBe<RangeRule>();
			rule.Max.ShouldEqual(5);
			rule.Min.ShouldBeNull();
		}

		[Test]
		public void Converts_LessThanOrEqual_to_RangeRule_for_decimal() {
			validator.RuleFor(x => x.Discount).LessThanOrEqualTo(5);

			var rules = provider.GetRulesFromType(typeof(Person));
			var rule = rules["Discount"].Single().ShouldBe<RangeRule>();
			rule.Max.ShouldEqual((decimal)5);
			rule.Min.ShouldBeNull();
		}

		[Test]
		public void Converts_LessThanOrEqual_to_RangeRule_for_string() {
			validator.RuleFor(x => x.Surname).LessThanOrEqualTo("x");

			var rules = provider.GetRulesFromType(typeof(Person));
			var rule = rules["Surname"].Single().ShouldBe<RangeRule>();
			rule.Max.ShouldEqual("x");
			rule.Min.ShouldBeNull();
		}

		[Test]
		public void Converts_LessThanOrEqual_to_RangeRule_for_date() {
			validator.RuleFor(x => x.DateOfBirth).LessThanOrEqualTo(new DateTime(1987, 4, 19));

			var rules = provider.GetRulesFromType(typeof(Person));
			var rule = rules["DateOfBirth"].Single().ShouldBe<RangeRule>();
			rule.Max.ShouldEqual(new DateTime(1987,4,19));
			rule.Min.ShouldBeNull();
		}

		[Test]
		public void Converts_GreaterThanOrEqual_to_RangeRule_for_int() {
			validator.RuleFor(x => x.Id).GreaterThanOrEqualTo(5);

			var rules = provider.GetRulesFromType(typeof(Person));
			var rule = rules["Id"].Single().ShouldBe<RangeRule>();
			rule.Min.ShouldEqual(5);
			rule.Max.ShouldBeNull();
		}

		[Test]
		public void Converts_GreaterThanOrEqual_to_RangeRule_for_decimal() {
			validator.RuleFor(x => x.Discount).GreaterThanOrEqualTo(5);

			var rules = provider.GetRulesFromType(typeof(Person));
			var rule = rules["Discount"].Single().ShouldBe<RangeRule>();
			rule.Min.ShouldEqual((decimal)5);
			rule.Max.ShouldBeNull();
		}

		[Test]
		public void Converts_GreaterThanOrEqual_to_RangeRule_for_string() {
			validator.RuleFor(x => x.Surname).GreaterThanOrEqualTo("x");

			var rules = provider.GetRulesFromType(typeof(Person));
			var rule = rules["Surname"].Single().ShouldBe<RangeRule>();
			rule.Min.ShouldEqual("x");
			rule.Max.ShouldBeNull();
		}

		[Test]
		public void Converts_GreaterThanOrEqual_to_RangeRule_for_date() {
			validator.RuleFor(x => x.DateOfBirth).GreaterThanOrEqualTo(new DateTime(1987, 4, 19));

			var rules = provider.GetRulesFromType(typeof(Person));
			var rule = rules["DateOfBirth"].Single().ShouldBe<RangeRule>();
			rule.Min.ShouldEqual(new DateTime(1987, 4, 19));
			rule.Max.ShouldBeNull();
		}

		[Test]
		public void Returns_empty_RuleSet_for_unknown_type() {
			provider.GetRulesFromType(typeof(object)).Keys.Count().ShouldEqual(0);
		}

		[Test]
		public void Correctly_retrieves_multiple_rules() {
			validator.RuleFor(x => x.Surname).NotNull().NotEqual(x => x.Forename);
			var rules = provider.GetRulesFromType(typeof(Person));
			rules["Surname"].Count().ShouldEqual(2);
			rules["Surname"].ElementAt(0).ShouldBe<RequiredRule>();
			rules["Surname"].ElementAt(1).ShouldBe<ComparisonRule>();
		}

	}
}