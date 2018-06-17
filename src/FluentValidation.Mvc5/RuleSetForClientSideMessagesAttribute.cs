namespace FluentValidation.Mvc {
	using System.Web;
	using System.Web.Mvc;

	/// <summary>
	/// Specifies which ruleset should be used when deciding which validators should be used to generate client-side messages.
	/// </summary>
	public class RuleSetForClientSideMessagesAttribute : ActionFilterAttribute {
		private const string key = "_FV_ClientSideRuleSet";
		private readonly string[] _ruleSets;

		public RuleSetForClientSideMessagesAttribute(string ruleSet) {
			_ruleSets = new[] { ruleSet };
		}

		public RuleSetForClientSideMessagesAttribute(params string[] ruleSets) {
			this._ruleSets = ruleSets;
		}

		public override void OnActionExecuting(ActionExecutingContext filterContext) {
			SetRulesetForClientValidation(filterContext.HttpContext, _ruleSets);
		}

		public static void SetRulesetForClientValidation(HttpContextBase context, string[] ruleSets) {
			context.Items[key] = ruleSets;
		}

		public static string[] GetRuleSetsForClientValidation(HttpContextBase context) {
			return context.Items[key] as string[] ?? new[] { "default" };
		}
	}
}