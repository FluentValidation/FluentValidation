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
// The latest version of this file can be found at https://github.com/JeremySkinner/FluentValidation
#endregion
namespace FluentValidation.Tests {
	using System.Collections.Generic;
	using System.Linq;
	using Validators;
	using Xunit;

	
	public class ForEachRuleTests {
		[Fact]
		public void Executes_rule_for_each_item_in_collection() {
			var validator = new TestValidator {
				v => v.RuleForEach(x => x.NickNames).NotNull()
			};

			var person = new Person {
				NickNames =  new[] { null, "foo", null }
			};

			var result = validator.Validate(person);
			result.Errors.Count.ShouldEqual(2);
		}

		[Fact]
		public void Correctly_gets_collection_indicies() {
			var validator = new TestValidator {
				v => v.RuleForEach(x => x.NickNames).NotNull()
			};

			var person = new Person {
				NickNames = new[] { null, "foo", null }
			};

			var result = validator.Validate(person);
			result.Errors[0].PropertyName.ShouldEqual("NickNames[0]");
			result.Errors[1].PropertyName.ShouldEqual("NickNames[2]");
		}

		[Fact]
		public void Executes_rule_for_each_item_in_collection_async()
		{
			var validator = new TestValidator {
				v => v.RuleForEach(x => x.NickNames).SetValidator(new MyAsyncNotNullValidator())
			};

			var person = new Person
			{
				NickNames = new[] { null, "foo", null }
			};

			var result = validator.ValidateAsync(person).Result;
			result.Errors.Count.ShouldEqual(2);
		}

		[Fact]
		public void Correctly_gets_collection_indicies_async()
		{
			var validator = new TestValidator {
				v => v.RuleForEach(x => x.NickNames).SetValidator(new MyAsyncNotNullValidator())
			};

			var person = new Person
			{
				NickNames = new[] { null, "foo", null }
			};

			var result = validator.ValidateAsync(person).Result;
			result.Errors[0].PropertyName.ShouldEqual("NickNames[0]");
			result.Errors[1].PropertyName.ShouldEqual("NickNames[2]");
		}

		class request {
			public Person person = null;
		}

		private class MyAsyncNotNullValidator : NotNullValidator {
			public override bool IsAsync { get { return true; } }
		}

		[Fact]
		public void Nested_collection_for_null_property_should_not_throw_null_reference() {
			var validator = new InlineValidator<request>();
			validator.When(r => r.person != null, () => {
				validator.RuleForEach(x => x.person.NickNames).NotNull();
			});

			var result = validator.Validate(new request());
			result.Errors.Count.ShouldEqual(0);
		}

		[Fact]
		public void Should_not_scramble_property_name_when_using_collection_validators_several_levels_deep() {
			var v = new ApplicationViewModelValidator();
			var result = v.Validate(new ApplicationViewModel());

			result.Errors.Single().PropertyName.ShouldEqual("TradingExperience[0].Questions[0].SelectedAnswerID");
		}


		public class ApplicationViewModel {
			public List<ApplicationGroup> TradingExperience { get; set; } = new List<ApplicationGroup> { new ApplicationGroup() };

		}

		public class ApplicationGroup {
			public List<Question> Questions = new List<Question> { new Question() };
		}

		public class Question {
			public int SelectedAnswerID { get; set; }
		}
		public class ApplicationViewModelValidator : AbstractValidator<ApplicationViewModel>  {
			public ApplicationViewModelValidator() {
				RuleForEach(x => x.TradingExperience)
					.SetValidator(new AppropriatenessGroupViewModelValidator());
			}
		}

		public class AppropriatenessGroupViewModelValidator : AbstractValidator<ApplicationGroup> {

			public AppropriatenessGroupViewModelValidator() {

				RuleForEach(m => m.Questions)
					.SetValidator(new AppropriatenessQuestionViewModelValidator());
			}
		}

		public class AppropriatenessQuestionViewModelValidator : AbstractValidator<Question> {
			public AppropriatenessQuestionViewModelValidator() {
				RuleFor(m => m.SelectedAnswerID)
					.SetValidator(new AppropriatenessAnswerViewModelRequiredValidator()); ;
			}
		}

		public class AppropriatenessAnswerViewModelRequiredValidator : PropertyValidator {

			public AppropriatenessAnswerViewModelRequiredValidator()
				: base("Error message here.") {
			}

			protected override bool IsValid(PropertyValidatorContext context) {
				return false;
			}
		}
		
	}
}