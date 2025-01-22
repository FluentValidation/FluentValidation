namespace FluentValidation.TestHelper;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Results;

public interface ITestValidationWith : ITestValidationContinuation {
}

public interface ITestValidationContinuation : IEnumerable<ValidationFailure> {
	IEnumerable<ValidationFailure> UnmatchedFailures { get; }
	IEnumerable<ValidationFailure> MatchedFailures { get; }
}

internal class TestValidationContinuation(IEnumerable<ValidationFailure> failures, ITestValidationContinuation parent = null)
	: ITestValidationContinuation, ITestValidationWith {
	private readonly List<Func<ValidationFailure,bool>> _predicates = new();

	public ITestValidationContinuation Parent { get; } = parent;

	public void ApplyPredicate(Func<ValidationFailure, bool> failurePredicate) {
		_predicates.Add(failurePredicate);
	}

	public IEnumerator<ValidationFailure> GetEnumerator() {
		return MatchedFailures.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return ((IEnumerable) MatchedFailures).GetEnumerator();
	}

	public IEnumerable<ValidationFailure> MatchedFailures {
		get {
			var matchedFailures = failures;
			foreach (var predicate in _predicates) {
				matchedFailures = matchedFailures.Where(predicate);
			}

			return matchedFailures;
		}
	}

	public IEnumerable<ValidationFailure> UnmatchedFailures {
		get {
			foreach (var failure in failures) {
				foreach (var predicate in _predicates) {
					if (!predicate(failure)) {
						yield return failure;
						break;
					}
				}
			}
		}
	}
}
