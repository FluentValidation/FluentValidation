namespace FluentValidation.Tests.AspNetCore.Controllers {
	using System.Collections.Generic;
	using FluentValidation.AspNetCore;
	using Microsoft.AspNetCore.Mvc;
	using System.Linq;
	
	public class TestController : Controller {
		public ActionResult SimpleFailure(SimpleModel model) {
			return TestResult();
		}

		public ActionResult Test1(TestModel test) {
			return TestResult();
		}

		public ActionResult Test1a(TestModel foo) {
			return TestResult();
		}

		public ActionResult Test2(TestModel2 test) {
			return Content(test == null ? "null" : "not null");
		}

		public ActionResult Test3(TestModel3 test) {
			return TestResult();
		}

		public ActionResult Test4(TestModel4 test) {
			return TestResult();
		}

		public ActionResult Test6(TestModel6 test) {
			return TestResult();
		}

		public ActionResult WithoutValidator(TestModelWithoutValidator test) {
			return TestResult();
		}

		public ActionResult TestModelWithOverridenMessageValueType(TestModelWithOverridenMessageValueType test) {
			return TestResult();
		}

		public ActionResult TestModelWithOverridenPropertyNameValueType(TestModelWithOverridenPropertyNameValueType test) {
			return TestResult();
		}

		public ActionResult RulesetTestModel(RulesetTestModel test) {
			return TestResult();
		}

		public ActionResult Test5(TestModel5 model) {
			return TestResult();
		}

		public ActionResult RulesetTest([CustomizeValidator(RuleSet = "Names")] RulesetTestModel test) {
			return TestResult();
		}

		public ActionResult PropertyTest([CustomizeValidator(Properties="Surname,Forename")]PropertiesTestModel test) {
			return TestResult();
		}

		public ActionResult InterceptorTest([CustomizeValidator(Interceptor = typeof(SimplePropertyInterceptor))] PropertiesTestModel test) {
			return TestResult();
		}

		public ActionResult ClearErrorsInterceptorTest([CustomizeValidator(Interceptor = typeof(ClearErrorsInterceptor))] PropertiesTestModel test) {
			return TestResult();
		}

		public ActionResult BuiltInInterceptorTest(PropertiesTestModel2 test) {
			return TestResult();
		}
		public ActionResult TwoParameters([CustomizeValidator(RuleSet = "Names")]RulesetTestModel first, RulesetTestModel second) {
			return TestResult();
		}

		public ActionResult Lifecycle(LifecycleTestModel model) {
			return TestResult();
		}

		public ActionResult Collection(List<CollectionTestModel> model) {
			return TestResult();
		}

		public ActionResult MultipleErrors(MultipleErrorsModel model) {
			return TestResult();
		}

		public ActionResult ModelThatimplementsIEnumerable(ModelThatImplementsIEnumerable model) {
			return TestResult();
		}

		public ActionResult MultipleValidationStrategies(MultiValidationModel model) {
			return TestResult();
		}

		public ActionResult MultipleValidationStrategies2(MultiValidationModel2 model) {
			return TestResult();
		}

		public ActionResult MultipleValidationStrategies3(MultiValidationModel3 model) {
			return TestResult();
		}

		public ActionResult DataAnnotations(DataAnnotationsModel model) {
			return TestResult();
		}

		public ActionResult ImplicitChildValidator(ParentModel model) {
			return TestResult();
		}

		public ActionResult ImplicitChildValidatorWithNullChild(ParentModel5 model) {
			return TestResult();
		}

		public ActionResult ImplementsIValidatableObject(ImplementsIValidatableObjectModel model) {
			return TestResult();
		}

		public ActionResult ImplicitChildImplementsIValidatableObject(ParentModel2 model) {
			return TestResult();
		}

		public ActionResult ImplicitChildWithDataAnnotations(ParentModel3 model) {
			return TestResult();
		}

		public ActionResult ImplicitAndExplicitChildValidator(ParentModel4 model) {
			return TestResult();
		}

		public ActionResult DictionaryParameter(Dictionary<int, TestModel> model) {
			return TestResult();
		}

		public IActionResult UsingDictionaryWithJsonBody([FromBody]Dictionary<int, TestModel5> model)
		{
			return TestResult();
		}

		public IActionResult UsingEnumerable([FromBody]IEnumerable<TestModel5> model)
		{
			return TestResult();
		}

		public ActionResult SkipsValidation([CustomizeValidator(Skip=true)] TestModel test) {
			return TestResult();
		}

		public ActionResult SkipsImplicitChildValidator([CustomizeValidator(Skip=true)] ParentModel model) {
			return TestResult();
		}

		private ActionResult TestResult() {
			var errors = new List<SimpleError>();

			foreach (var pair in ModelState) {
				foreach (var error in pair.Value.Errors) {
					errors.Add(new SimpleError {Name = pair.Key, Message = error.ErrorMessage});
				}
			}

			return Json(errors);
		}
	}


	public class SimpleError {
		public string Name { get; set; }
		public string Message { get; set; }

		public override string ToString() {
			return $"Property: {Name} Message: {Message}";
		}
	}

	public class SimpleModel {
		public string Name { get; set; }
		public int Id { get; set; }
	}

	public static class TestHelper {
		public static bool IsValid(this List<SimpleError> errors) {
			return errors.Count == 0;
		}

		public static bool IsValidField(this List<SimpleError> errors, string name) {
			return errors.All(x => x.Name != name);
		}

		public static string GetError(this List<SimpleError> errors, string name) {
			return errors.Where(x => x.Name == name).Select(x => x.Message).SingleOrDefault() ?? "";
		}
	}
}