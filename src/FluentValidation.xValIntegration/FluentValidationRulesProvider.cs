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

		public FluentValidationRulesProvider(IValidatorFactory factory) {
			this.factory = factory;

			ruleEmitters.AddSingle<INotNullValidator>(x => new RequiredRule());
			ruleEmitters.AddSingle<INotEmptyValidator>(x => new RequiredRule());
			ruleEmitters.AddSingle<ILengthValidator>(x => new StringLengthRule(x.Min, x.Max));
			ruleEmitters.AddSingle<IEmailValidator>(x => new DataTypeRule(DataTypeRule.DataType.EmailAddress));
			ruleEmitters.AddSingle<IRegularExpressionValidator>(x => new RegularExpressionRule(x.Expression));

			ruleEmitters.AddSingle<IComparisonValidator>(x => {
				if (x.Comparison == Comparison.Equal && x.MemberToCompare != null)
					return new ComparisonRule(x.MemberToCompare.Name, ComparisonRule.Operator.Equals);
				if (x.Comparison == Comparison.NotEqual && x.MemberToCompare != null)
					return new ComparisonRule(x.MemberToCompare.Name, ComparisonRule.Operator.DoesNotEqual);
				if (x.Comparison == Comparison.GreaterThanOrEqual && x.ValueToCompare != null)
					return GenerateComparisonRule(x.ValueToCompare, x.Comparison);
				if (x.Comparison == Comparison.LessThanOrEqual && x.ValueToCompare != null)
					return GenerateComparisonRule(x.ValueToCompare, x.Comparison);
				return null;
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

			if(validator != null) {
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