namespace FluentValidation.MvcIntegrationDemo.Controllers {
	using System.Web.Mvc;
	using Models;

	public class PeopleController : Controller {
		public ActionResult Index() {
			ViewData["valid"] = ModelState.IsValid;
			return View();
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Index(Person person) {
			ViewData["valid"] = ModelState.IsValid;
			return View(person);
		}
	}
}