namespace FluentValidation.Tests.Mvc6.Controllers {
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    public class TestController : Controller {

        public ActionResult SimpleFailure(SimpleModel model) {
            return TestResult();
        }

        public ActionResult Test1(TestModel test) {
            return TestResult();
        }

        public ActionResult Test1a(TestModel foo)
        {
            return TestResult();
        }

        public ActionResult Test3(TestModel3 test) {
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

        private ActionResult TestResult() {
            var errors = new List<SimpleError>();

            foreach (var pair in ModelState)
            {
                foreach (var error in pair.Value.Errors)
                {
                    errors.Add(new SimpleError { Name = pair.Key, Message = error.ErrorMessage });
                }
            }

            return Json(errors);
        }
    }

  

    public class SimpleError {
        public string Name { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
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

        public static string GetError(this List<SimpleError> errors, string name)
        {
            return errors.Where(x => x.Name == name).Select(x => x.Message).SingleOrDefault() ?? "";
        }
    }
}