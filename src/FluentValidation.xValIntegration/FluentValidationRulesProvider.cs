namespace FluentValidation.xValIntegration {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Validators;
	using xVal.RuleProviders;
	using xVal.Rules;

	/// <summary>
	/// Rules provider that provides xVal integration for FluentValidation.
	/// </summary>
	public class FluentValidationRulesProvider : CachingRulesProvider {
		readonly IValidatorFactory factory;
		readonly RuleEmitterList<IPropertyValidator> ruleEmitters = new RuleEmitterList<IPropertyValidator>();

		private void CopyErrorMessages(IPropertyValidator source, Rule destination) {
			if (source.CustomMessageFormatArguments.Count == 0) {
				if (source.ErrorMessageSource.ResourceType != null && source.ErrorMessageSource.ResourceName != null) {
					destination.ErrorMessageResourceName = source.ErrorMessageSource.ResourceName;
					destination.ErrorMessageResourceType = source.ErrorMessageSource.ResourceType;
				}
				else {
					destination.ErrorMessage = source.ErrorMessageSource.GetString();
				}
			}
		}

		public FluentValidationRulesProvider(IValidatorFactory factory) {
			this.factory = factory;

			ruleEmitters.AddSingle<INotNullValidator>(x => {
				Rule rule = new RequiredRule();
				CopyErrorMessages(x, rule);
				return rule;
			});

			ruleEmitters.AddSingle<INotEmptyValidator>(x => {
				Rule rule = new RequiredRule();
				CopyErrorMessages(x, rule);
				return rule;
			});

			ruleEmitters.AddSingle<ILengthValidator>(x => {
				Rule rule = new StringLengthRule(x.Min, x.Max);
				CopyErrorMessages(x, rule);
				return rule;
			});

			ruleEmitters.AddSingle<IEmailValidator>(x => {
				Rule rule = new DataTypeRule(DataTypeRule.DataType.EmailAddress);
				CopyErrorMessages(x, rule);
				return rule;
			});

			ruleEmitters.AddSingle<IRegularExpressionValidator>(x => {
				Rule rule = new RegularExpressionRule(x.Expression);
				CopyErrorMessages(x, rule);
				return rule;
			});

			ruleEmitters.AddSingle<IComparisonValidator>(x => {
				Rule rule = null;

				if (x.Comparison == Comparison.Equal && x.MemberToCompare != null)
					rule = new ComparisonRule(x.MemberToCompare.Name, ComparisonRule.Operator.Equals);
				if (x.Comparison == Comparison.NotEqual && x.MemberToCompare != null)
					rule = new ComparisonRule(x.MemberToCompare.Name, ComparisonRule.Operator.DoesNotEqual);
				if (x.Comparison == Comparison.GreaterThanOrEqual && x.ValueToCompare != null)
					rule = GenerateComparisonRule(x.ValueToCompare, x.Comparison);
				if (x.Comparison == Comparison.LessThanOrEqual && x.ValueToCompare != null)
					rule = GenerateComparisonRule(x.ValueToCompare, x.Comparison);
				if (rule != null) {
					CopyErrorMessages(x, rule);
				}
				return rule;
			});

			//The rule for DelegatingValidator *must* be last
			ruleEmitters.AddMultiple<IDelegatingValidator>(x => {
				var delegatingValidator = x as IDelegatingValidator;
				if (delegatingValidator != null) {
					return ruleEmitters.EmitRules(delegatingValidator.InnerValidator);
				}
				return null;
			});
		}

		RangeRule GenerateComparisonRule(object valueToCompare, Comparison comparison) {
			if (comparison == Comparison.GreaterThanOrEqual) {
				return BuildRangeRule<decimal>(valueToCompare, x => new RangeRule(x, null))
					?? BuildRangeRule<DateTime>(valueToCompare, x => new RangeRule(x, null))
					?? BuildRangeRule<int>(valueToCompare, x => new RangeRule(x, null))
					?? BuildRangeRule<string>(valueToCompare, x => new RangeRule(x, null));
			}

			if (comparison == Comparison.LessThanOrEqual) {
				return BuildRangeRule<decimal>(valueToCompare, x => new RangeRule(null, x))
					?? BuildRangeRule<DateTime>(valueToCompare, x => new RangeRule(null, x))
					?? BuildRangeRule<int>(valueToCompare, x => new RangeRule(null, x))
					?? BuildRangeRule<string>(valueToCompare, x => new RangeRule(null, x));
			}

			return null;
		}

		RangeRule BuildRangeRule<T>(object valueToCompare, Func<T, RangeRule> ruleBuilder) {
			if (valueToCompare is T) {
				return ruleBuilder((T)valueToCompare);
			}

			return null;
		}

		protected override RuleSet GetRulesFromTypeCore(Type type) {
			var validator = factory.GetValidator(type);

			if (validator != null) {
				var descriptor = validator.CreateDescriptor();

				var rules = from memberWithValidators in descriptor.GetMembersWithValidators()
							from xValRule in memberWithValidators.SelectMany(x => ConvertToXValRules(x))
							select new { MemberName = memberWithValidators.Key, Rule = xValRule };

				return new RuleSet(rules.ToLookup(x => x.MemberName, x => x.Rule));
			}

			return RuleSet.Empty;
		}

		IEnumerable<Rule> ConvertToXValRules(IPropertyValidator val) {
			return ruleEmitters.EmitRules(val);
		}
	}
}