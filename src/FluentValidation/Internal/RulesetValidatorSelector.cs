namespace FluentValidation.Internal {
	using System;
	using System.Linq;

	public class RulesetValidatorSelector : IValidatorSelector {
		readonly string[] rulesetsToExecute;

		public RulesetValidatorSelector(params string[] rulesetsToExecute) {
			this.rulesetsToExecute = rulesetsToExecute;
		}

		public bool CanExecute(PropertyRule rule, string propertyPath, ValidationContext context) {
			if (string.IsNullOrEmpty(rule.RuleSet) && rulesetsToExecute.Length == 0) return true;
			if (!string.IsNullOrEmpty(rule.RuleSet) && rulesetsToExecute.Length > 0 && rulesetsToExecute.Contains(rule.RuleSet)) return true;

			return false;
		}
	}
}