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
namespace FluentValidation.Tests.WebApi {
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Http;
	using System.Web.Http.Results;

	public class TestController : ApiController {
        [HttpPost]
        public IHttpActionResult TestModel10(TestModel10 model)
        {
            return OutputErrors();
        }

        [HttpPost]
		public IHttpActionResult TestModel9(TestModel9 model) {
			return OutputErrors();
		}

		[HttpPost]
		public IHttpActionResult TestModel8(TestModel8 model) {
			return OutputErrors();
		}

		[HttpPost]
		public IHttpActionResult TestModel7(TestModel7 model) {
			return OutputErrors();
		}

		[HttpPost]
		public IHttpActionResult TestModel6(TestModel6 model) {
			return OutputErrors();
		}

		[HttpPost]
		public IHttpActionResult TestModel5(TestModel5 model) {
			return OutputErrors();
		}

		[HttpPost]
		public IHttpActionResult TestModel4(TestModel4 model) {
			return OutputErrors();
		}
		[HttpPost]
		public IHttpActionResult TestModel3(TestModel3 model) {
			return OutputErrors();
		}

		[HttpPost]
		public IHttpActionResult TestModel2(TestModel2 model) {
			return OutputErrors();
		}

		[HttpPost]
		public IHttpActionResult TestModel(TestModel model) {
			return OutputErrors();
		}

		[HttpPost]
		public IHttpActionResult TestModelWithoutValidator(TestModelWithoutValidator model) {
			return OutputErrors();
		}

		private JsonResult<List<SimpleError>> OutputErrors() {
			var q = from x in ModelState
				from err in x.Value.Errors
				let message = string.IsNullOrEmpty(err.ErrorMessage) ? err.Exception.Message : err.ErrorMessage
				select new SimpleError {Message = message, Property = x.Key};

			return Json(q.ToList());
		} 
	}

	public class SimpleError {
		public string Property { get; set; }
		public string Message { get; set; }
	}

	public static class ErrorExtensions {
		public static bool IsValid(this List<SimpleError> list) {
			return !list.Any();
		}

		public static bool IsValidField(this List<SimpleError> list, string property) {
			return !list.Any(x => x.Property == property);
		}

		public static string GetMessage(this List<SimpleError> list, string property) {
			return list.Where(x => x.Property == property).Select(x => x.Message).FirstOrDefault();
		}
	}
}