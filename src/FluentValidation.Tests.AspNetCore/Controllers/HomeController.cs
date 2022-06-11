namespace FluentValidation.Tests.Controllers;

using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller {
	public ActionResult Index() => Content("Test");
}
