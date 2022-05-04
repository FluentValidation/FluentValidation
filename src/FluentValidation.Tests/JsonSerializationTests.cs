namespace FluentValidation.Tests {
	using System.Text.Json;
	using Results;
	using Xunit;

	public class JsonSerializationTests {
		[Fact]
		public void Consecutive_Serialize_Deserialization() {
			var validationResult = new ValidationResult {
				Errors = {
					new ValidationFailure("MyProperty1", "Invalid MyProperty1"),
					new ValidationFailure("MyProperty2", "Invalid MyProperty2"),
					new ValidationFailure("MyProperty3", "Invalid MyProperty3")
				}
			};
			// System.Text.Json
			var serialized1 = JsonSerializer.Serialize(validationResult);
			var deserialized1 = JsonSerializer.Deserialize<ValidationResult>(serialized1);

			// Newtonsoft.Json
			var serialized2 = Newtonsoft.Json.JsonConvert.SerializeObject(validationResult);
			var deserialized2 = Newtonsoft.Json.JsonConvert.DeserializeObject<ValidationResult>(serialized2);

			Assert.NotNull(deserialized1);
			Assert.Equal(deserialized1.IsValid, deserialized2.IsValid);
			Assert.Equal(deserialized1.Errors.Count, deserialized2.Errors.Count);
			for (var i = 0; i < deserialized1.Errors.Count; i++) {
				Assert.Equal(deserialized1.Errors[i].PropertyName, deserialized2.Errors[i].PropertyName);
				Assert.Equal(deserialized1.Errors[i].ErrorMessage, deserialized2.Errors[i].ErrorMessage);
			}
		}
	}
}
