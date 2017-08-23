namespace FluentValidation.AspNetCore {
	using System;
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc.Filters;

	/// <summary>
	/// Specifies which ruleset should be used when deciding which validators should be used to generate client-side messages.
	/// </summary>
	public class RuleSetForClientSideMessagesAttribute : ActionFilterAttribute {
		private const string key = "_FV_ClientSideRuleSet";
		string[] ruleSets;

		public RuleSetForClientSideMessagesAttribute(string ruleSet) {
			ruleSets = new[] { ruleSet };
		}

		public RuleSetForClientSideMessagesAttribute(params string[] ruleSets) {
			this.ruleSets = ruleSets;
		}

		public override void OnActionExecuting(ActionExecutingContext filterContext) {
			SetRulesetForClientValidation(filterContext.HttpContext, ruleSets);
		}

		public static void SetRulesetForClientValidation(HttpContext context, string[] ruleSets) {
			context.Items[key] = ruleSets;
		}

		public static string[] GetRuleSetsForClientValidation(HttpContext context) {
			// If the httpContext is null (for example, if IHttpContextProvider hasn't been registered) then throw an error

			if (context == null) {
				throw new Exception("RuleSetForClientSideMessagesAttribute without an HttpContext. Please ensure that the IHttpContextProvider has been registered in your application startup routine.");
			}

			return context.Items[key] as string[] ?? new string[] { null };
		}
	}
}