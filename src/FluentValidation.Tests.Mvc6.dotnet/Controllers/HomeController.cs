namespace FluentValidation.Tests.Mvc6.Controllers {
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller{
        public ActionResult Index() {
            return Content("Test");
        }
    }
}