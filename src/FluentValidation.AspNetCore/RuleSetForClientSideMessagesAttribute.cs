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
			var contextAccessor = filterContext.HttpContext.RequestServices.GetService(typeof(IHttpContextAccessor));

			if(contextAccessor == null) {
				throw new InvalidOperationException("Cannot use the RuleSetForClientSideMessagesAttribute unless the IHttpContextAccessor is registered with the service provider. Make sure the provider is registered by calling services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); in your Startup class's ConfigureServices method");
			}

			SetRulesetForClientValidation(filterContext.HttpContext, ruleSets);
		}

		public static void SetRulesetForClientValidation(HttpContext context, string[] ruleSets) {
			context.Items[key] = ruleSets;
		}

		public static string[] GetRuleSetsForClientValidation(HttpContext context) {
			// If the httpContext is null (for example, if IHttpContextProvider hasn't been registered) then just assume default ruleset.
			// This is OK because if we're actually using the attribute, the OnActionExecuting will have caught the fact that the provider is not registered. 

			if (context?.Items != null && context.Items.ContainsKey(key)) {
				return context?.Items[key] as string[] ?? new string[] { null };

			}
			return new string[] {null};
		}
	}
}