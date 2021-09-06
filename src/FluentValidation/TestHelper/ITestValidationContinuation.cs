namespace FluentValidation.TestHelper {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using Results;

	public interface ITestValidationWith : ITestValidationContinuation {
	}

	public interface ITestValidationContinuation : IEnumerable<ValidationFailure> {
		IEnumerable<ValidationFailure> UnmatchedFailures { get; }
	}

	internal class TestValidationContinuation : ITestValidationContinuation, ITestValidationWith {
		private readonly IEnumerable<ValidationFailure> _allFailures;
		private readonly List<Func<ValidationFailure,bool>> _predicates;

		public static TestValidationContinuation Create(IEnumerable<ValidationFailure> failures) =>
			new TestValidationContinuation(failures);

		public static TestValidationContinuation Create(ITestValidationContinuation continuation) {
			if (continuation is TestValidationContinuation instance)
				return instance;
			var allFailures = continuation.Union(continuation.UnmatchedFailures);
			instance = new TestValidationContinuation(allFailures);
			instance.ApplyPredicate(failure => !continuation.UnmatchedFailures.Contains(failure));
			return instance;
		}

		private TestValidationContinuation(IEnumerable<ValidationFailure> failures) {
			_allFailures = failures;
			_predicates = new List<Func<ValidationFailure, bool>>();
		}

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
				var matchedFailures = _allFailures;
				foreach (var predicate in _predicates) {
					matchedFailures = matchedFailures.Where(predicate);
				}

				return matchedFailures;
			}
		}

		public IEnumerable<ValidationFailure> UnmatchedFailures {
			get {
				foreach (var failure in _allFailures) {
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
}
