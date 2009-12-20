namespace FluentValidation.Internal {
	using System.Collections.Generic;

	public interface IValidationRuleCollection<T> : IEnumerable<IValidationRule<T>>, IValidationRule<T> {
		
	}
}