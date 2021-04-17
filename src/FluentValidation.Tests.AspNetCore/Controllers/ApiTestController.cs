namespace FluentValidation.Tests.AspNetCore.Controllers {
	using Microsoft.AspNetCore.Mvc;

	[ApiController]
	[Route("[controller]")]
	public class ApiTestController : Controller {

		[HttpPost]
		public ActionResult Create(TestModel test) {
			// Because this is an ApiController, the ModelStateInvalidFilter will prevent
			// this action from running and will return an automatically serialized ModelState response.
			return Ok();
		}

	}
}
