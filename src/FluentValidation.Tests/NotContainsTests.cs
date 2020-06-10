using System.Collections.Generic;
using Xunit;

namespace FluentValidation.Tests {
  public class NotContainsTests {
    public NotContainsTests() {
      CultureScope.SetDefaultCulture();
    }

    [Fact]
    public void When_there_is_a_string_value_notcontains_then_the_validator_should_fail() {
      var validator = new TestValidator {
        v => v.RuleFor(x => x.Surname).NotContains("helloworld")
      };

      var result = validator.Validate(new Person { Surname = "test" });
      result.IsValid.ShouldBeFalse();
    }

    [Fact]
    public void When_there_is_a_string_value_contains_then_the_validator_should_pass() {
      var validator = new TestValidator {
        v => v.RuleFor(x => x.Surname).NotContains("helloworld")
      };

      var result = validator.Validate(new Person { Surname = "helloworld" });
      result.IsValid.ShouldBeTrue();
    }


    [Fact]
    public void When_there_is_a_list_double_value_contains_then_the_validator_should_fail() {
      var validator = new TestValidator {
        v => v.RuleFor(x => x.Age).NotContains(new List<double>{ 30,31,32,33 })
      };

      var result = validator.Validate(new Person { Age = 29 });
      result.IsValid.ShouldBeFalse();
    }

    [Fact]
    public void When_there_is_a_list_double_value_contains_then_the_validator_should_pass() {
      var validator = new TestValidator {
        v => v.RuleFor(x => x.Age).NotContains(new List<double>{ 30,31,32,33 })
      };

      var result = validator.Validate(new Person { Age = 31 });
      result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void When_there_is_a_list_string_value_contains_then_the_validator_should_fail() {
      var validator = new TestValidator {
        v => v.RuleFor(x => x.Surname).NotContains(new List<string>{ "hello","world" })
      };

      var result = validator.Validate(new Person { Surname = "helloworld" });
      result.IsValid.ShouldBeFalse();
    }

    [Fact]
    public void When_there_is_a_list_string_value_contains_then_the_validator_should_pass() {
      var validator = new TestValidator {
        v => v.RuleFor(x => x.Surname).NotContains(new List<string>{ "hello","world" })
      };

      var result = validator.Validate(new Person { Surname = "hello" });
      result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void When_there_is_a_array_value_contains_then_the_validator_should_fail() {
      var validator = new TestValidator {
        v => v.RuleFor(x => x.Surname).NotContains(new string[2]{ "hello","world"})
      };

      var result = validator.Validate(new Person { Surname = "helloworld" });
      result.IsValid.ShouldBeFalse();
    }

    [Fact]
    public void When_there_is_a_array_value_contains_then_the_validator_should_pass() {
      var validator = new TestValidator {
        v => v.RuleFor(x => x.Surname).NotContains(new string[2]{ "hello","world"})
      };

      var result = validator.Validate(new Person { Surname = "hello" });
      result.IsValid.ShouldBeTrue();
    }


    [Fact]
    public void When_there_is_a_enumarable_value_contains_then_the_validator_should_fail() {
      IEnumerable<string> e1 = new string[] { "hello", "world" };
      var validator = new TestValidator {
        v => v.RuleFor(x => x.Surname).NotContains(e1)
      };

      var result = validator.Validate(new Person { Surname = "helloworld" });
      result.IsValid.ShouldBeFalse();
    }

    [Fact]
    public void When_there_is_a_enumarable_value_contains_then_the_validator_should_pass() {
      IEnumerable<string> e1 = new string[] { "hello", "world" };
      var validator = new TestValidator {
        v => v.RuleFor(x => x.Surname).NotContains(e1)
      };

      var result = validator.Validate(new Person { Surname = "hello" });
      result.IsValid.ShouldBeTrue();
    }
  }
}
