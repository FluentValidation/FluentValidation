namespace FluentValidation.Tests {
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using AspNetCore;
	using AspNetCore.Controllers;
	using FluentValidation.AspNetCore;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.RazorPages;

	[IgnoreAntiforgeryToken(Order = 1001)]
	public class TestPageModel : PageModel {

		[BindProperty]
		public TestModel Test { get; set; }

		public Task<IActionResult> OnPostAsync() {
			return Task.FromResult(TestResult());
		}

		private IActionResult TestResult() {
			var errors = new List<SimpleError>();

			foreach (var pair in ModelState) {
				foreach (var error in pair.Value.Errors) {
					errors.Add(new SimpleError { Name = pair.Key, Message = error.ErrorMessage });
				}
			}

			return new JsonResult(errors);
		}
	}

	[IgnoreAntiforgeryToken(Order = 1001)]
	public class RulesetTestPageModel : PageModel {

		[BindProperty]
		[CustomizeValidator(RuleSet = "Names")]
		public RulesetTestModel Test { get; set; }

		public Task<IActionResult> OnPostAsync() {
			return Task.FromResult(TestResult());
		}

		private IActionResult TestResult() {
			var errors = new List<SimpleError>();

			foreach (var pair in ModelState) {
				foreach (var error in pair.Value.Errors) {
					errors.Add(new SimpleError { Name = pair.Key, Message = error.ErrorMessage });
				}
			}

			return new JsonResult(errors);
		}
	}

	[IgnoreAntiforgeryToken(Order = 1001)]
	public class TestPageModelWithPrefix : PageModel {

		[BindProperty(Name = "Test")]
		public TestModel Test { get; set; }

		public Task<IActionResult> OnPostAsync() {
			return Task.FromResult(TestResult());
		}

		private IActionResult TestResult() {
			var errors = new List<SimpleError>();

			foreach (var pair in ModelState) {
				foreach (var error in pair.Value.Errors) {
					errors.Add(new SimpleError { Name = pair.Key, Message = error.ErrorMessage });
				}
			}

			return new JsonResult(errors);
		}
	}

	[IgnoreAntiforgeryToken(Order = 1001)]
	public class TestPageModelWithDefaultRuleSet : PageModel {

		[BindProperty(Name = "Test")]
		public ClientsideRulesetModel Test { get; set; }

		public IActionResult OnGet() => Page();
	}

	[IgnoreAntiforgeryToken(Order = 1001)]
	[RuleSetForClientSideMessages("Foo")]
	public class TestPageModelWithSpecifiedRuleSet : PageModel {

		[BindProperty(Name = "Test")]
		public ClientsideRulesetModel Test { get; set; }

		public IActionResult OnGet() => Page();
	}

	[IgnoreAntiforgeryToken(Order = 1001)]
	[RuleSetForClientSideMessages("Foo", "Bar")]
	public class TestPageModelWithMultipleRuleSets : PageModel {

		[BindProperty(Name = "Test")]
		public ClientsideRulesetModel Test { get; set; }

		public IActionResult OnGet() => Page();
	}

	[IgnoreAntiforgeryToken(Order = 1001)]
	[RuleSetForClientSideMessages("Foo", "default")]
	public class TestPageModelWithDefaultAndSpecifiedRuleSet : PageModel {

		[BindProperty(Name = "Test")]
		public ClientsideRulesetModel Test { get; set; }

		public IActionResult OnGet() => Page();
	}

	[IgnoreAntiforgeryToken(Order = 1001)]
	public class TestPageModelWithRuleSetForHandlers : PageModel {

		[BindProperty(Name = "Test")]
		public ClientsideRulesetModel Test { get; set; }

		public IActionResult OnGetDefault() => Page();

#if NETCOREAPP3_1 || NET5_0
		public IActionResult OnGetSpecified() {
			PageContext.SetRulesetForClientsideMessages("Foo");
			return Page();
		}

		public IActionResult OnGetMultiple() {
			PageContext.SetRulesetForClientsideMessages("Foo", "Bar");
			return Page();
		}

		public IActionResult OnGetDefaultAndSpecified() {
			PageContext.SetRulesetForClientsideMessages("Foo", "default");
			return Page();
		}
#endif
	}
}
