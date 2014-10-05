namespace FluentValidation.Mvc {
#if !CoreCLR
    using System.Web;
    using System.Web.Mvc;
#else
    using Microsoft.AspNet.Mvc;
    using Microsoft.AspNet.Http;
#endif

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
			filterContext.HttpContext.Items[key] = ruleSets;
		}

#if !CoreCLR
        public static string[] GetRuleSetsForClientValidation(HttpContextBase httpContext) {
#else
        public static string[] GetRuleSetsForClientValidation(HttpContext httpContext) { 
#endif
            return httpContext.Items[key] as string[] ?? new string[] { null };
        }
    }
}