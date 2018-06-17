namespace FluentValidation.AspNetCore {
	using System;
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc.Filters;

	/// <summary>
	/// Specifies which ruleset should be used when deciding which validators should be used to generate client-side messages.
	/// </summary>
	public class RuleSetForClientSideMessagesAttribute : ActionFilterAttribute {
		private const string _key = "_FV_ClientSideRuleSet";
		private readonly string[] _ruleSets;

		public RuleSetForClientSideMessagesAttribute(string ruleSet) {
			_ruleSets = new[] { ruleSet };
		}

		public RuleSetForClientSideMessagesAttribute(params string[] ruleSets) {
			this._ruleSets = ruleSets;
		}

		public override void OnActionExecuting(ActionExecutingContext filterContext) {
			var contextAccessor = filterContext.HttpContext.RequestServices.GetService(typeof(IHttpContextAccessor));

			if(contextAccessor == null) {
				throw new InvalidOperationException("Cannot use the RuleSetForClientSideMessagesAttribute unless the IHttpContextAccessor is registered with the service provider. Make sure the provider is registered by calling services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); in your Startup class's ConfigureServices method");
			}

			SetRulesetForClientValidation(filterContext.HttpContext, _ruleSets);
		}

		
		/// <summary>
		/// Allows the ruleset used for generating clientside metadata to be overriden.
		/// By default, only rules not in a ruleset will be used.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="ruleSets"></param>
		public static void SetRulesetForClientValidation(HttpContext context, string[] ruleSets) {
			context.Items[_key] = ruleSets;
		}

		/// <summary>
		/// Gets the rulesets used to generate clientside validation metadata.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public static string[] GetRuleSetsForClientValidation(HttpContext context) {
			// If the httpContext is null (for example, if IHttpContextProvider hasn't been registered) then just assume default ruleset.
			// This is OK because if we're actually using the attribute, the OnActionExecuting will have caught the fact that the provider is not registered. 

			if (context?.Items != null && context.Items.ContainsKey(_key)) {
				return context?.Items[_key] as string[] ?? new[] { "default" };

			}
			return new[] {"default"};
		}
	}
}