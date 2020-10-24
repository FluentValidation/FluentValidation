using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace FluentValidation.Internal {

	/// <summary>
	/// Collection specific configuration.
	/// There is no need to be implemented in the user code.
	/// </summary>
	public interface ICollectionPropertyRule<T, TElement> : IValidationRule<T, TElement> {

		/// <summary>
		/// Filter that should include/exclude items in the collection.
		/// </summary>
		Func<TElement, bool> Filter { get; set; }

		/// <summary>
		/// Constructs the indexer in the property name associated with the error message.
		/// By default this is "[" + index + "]"
		/// </summary>
		Func<object, IEnumerable<TElement>, TElement, int, string> IndexBuilder { get; set; }
	}
}
