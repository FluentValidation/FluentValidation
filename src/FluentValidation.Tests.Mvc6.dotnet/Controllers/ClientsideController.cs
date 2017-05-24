namespace FluentValidation.Tests.AspNetCore.Controllers {
	using Microsoft.AspNetCore.Mvc;

	public class ClientsideController : Controller {
		public ActionResult Inputs() {
			return View();
		}
	}
}