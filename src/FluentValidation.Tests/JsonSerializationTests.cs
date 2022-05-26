namespace FluentValidation.Tests;

using System.Text.Json;
using Results;
using Xunit;

public class JsonSerializationTests {

	[Fact]
	public void SystemTextJson_deserialization_should_be_consistent_with_newtonsoft() {
		var validationResult = new ValidationResult {
			Errors = {
				new ValidationFailure("MyProperty1", "Invalid MyProperty1"),
				new ValidationFailure("MyProperty2", "Invalid MyProperty2"),
			},
			RuleSetsExecuted = new[] { "Test1" }
		};

		// System.Text.Json
		var serialized1 = JsonSerializer.Serialize(validationResult);
		var deserialized1 = JsonSerializer.Deserialize<ValidationResult>(serialized1);

		// Newtonsoft.Json
		var serialized2 = Newtonsoft.Json.JsonConvert.SerializeObject(validationResult);
		var deserialized2 = Newtonsoft.Json.JsonConvert.DeserializeObject<ValidationResult>(serialized2);

		deserialized1.IsValid.ShouldBeFalse();
		deserialized2.IsValid.ShouldBeFalse();

		deserialized1.Errors.Count.ShouldEqual(2);
		deserialized2.Errors.Count.ShouldEqual(2);

		deserialized1.Errors[0].PropertyName.ShouldEqual("MyProperty1");
		deserialized2.Errors[0].PropertyName.ShouldEqual("MyProperty1");

		deserialized1.Errors[1].PropertyName.ShouldEqual("MyProperty2");
		deserialized2.Errors[1].PropertyName.ShouldEqual("MyProperty2");

		deserialized1.Errors[0].ErrorMessage.ShouldEqual("Invalid MyProperty1");
		deserialized2.Errors[0].ErrorMessage.ShouldEqual("Invalid MyProperty1");

		deserialized1.Errors[1].ErrorMessage.ShouldEqual("Invalid MyProperty2");
		deserialized2.Errors[1].ErrorMessage.ShouldEqual("Invalid MyProperty2");

		deserialized1.RuleSetsExecuted.Length.ShouldEqual(1);
		deserialized2.RuleSetsExecuted.Length.ShouldEqual(1);

		deserialized1.RuleSetsExecuted[0].ShouldEqual("Test1");
		deserialized2.RuleSetsExecuted[0].ShouldEqual("Test1");
	}
}
