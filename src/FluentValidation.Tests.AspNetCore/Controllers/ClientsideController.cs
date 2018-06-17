namespace FluentValidation.Tests.AspNetCore.Controllers {
	using FluentValidation.AspNetCore;
	using Microsoft.AspNetCore.Mvc;

	public class ClientsideController : Controller {
		public ActionResult Inputs() {
			return View();
		}

		public ActionResult DefaultRuleset() {
			return View("RuleSet");
		}

		[RuleSetForClientSideMessages("Foo")]
		public ActionResult SpecifiedRuleset() {
			return View("RuleSet");
		}

		[RuleSetForClientSideMessages("Foo", "Bar")]
		public ActionResult MultipleRulesets() {
			return View("RuleSet");
		}

		[RuleSetForClientSideMessages("Foo", "default")]
		public ActionResult DefaultAndSpecified() {
			return View("RuleSet");
		}


	}
}