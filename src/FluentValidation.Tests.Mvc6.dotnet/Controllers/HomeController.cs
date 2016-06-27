namespace FluentValidation.Tests.AspNetCore.Controllers {
	using Microsoft.AspNetCore.Mvc;

	public class HomeController : Controller{
        public ActionResult Index() {
            return Content("Test");
        }
    }
}