namespace FluentValidation.Internal {
	using System;

	public class RulesetValidatorSelector : IValidatorSelector {
		readonly string rulesetToExecute;

		public RulesetValidatorSelector(string rulesetToExecute) {
			this.rulesetToExecute = rulesetToExecute;
		}

		public bool CanExecute(PropertyRule rule, string propertyPath, ValidationContext context) {
			if (string.IsNullOrEmpty(rule.RuleSet) && string.IsNullOrEmpty(rulesetToExecute)) return true;
			if (!string.IsNullOrEmpty(rulesetToExecute) && rulesetToExecute == rule.RuleSet) return true;

			return false;
		}
	}
}