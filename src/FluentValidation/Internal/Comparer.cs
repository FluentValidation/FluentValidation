using System;

namespace FluentValidation.Internal
{
	public static class Comparer {
		public static bool TryCompare(IComparable value, IComparable valueToCompare, out int result) {
			try {
				Compare(value, valueToCompare, out result);
				return true;
			}
			catch {
			}

			result = 0;
			return false;
		}

		/// <summary>
		/// Tries to do a proper comparison but may fail.
		/// First it tries the default comparison, if this fails, it will see 
		/// if the values are fractions. If they are, then it does a double 
		/// comparison, otherwise it does a long comparison.
		/// </summary>
		private static void Compare(IComparable value, IComparable valueToCompare, out int result) {
			try {
				// try default (will work on same types)
				result = value.CompareTo(valueToCompare);
			} catch {
				// attempt to to value type comparison
				if (value is decimal || valueToCompare is decimal ||
					value is double || valueToCompare is double ||
					value is float || valueToCompare is float) {
					// we are comparing a decimal/double/float, then compare using doubles
					result = Convert.ToDouble(value).CompareTo(Convert.ToDouble(valueToCompare));
				}
				else {
					// use long integer
					result = ((long)value).CompareTo((long)valueToCompare);
				}
			}
		}

		public static int GetComparisonResult(IComparable value, IComparable valueToCompare) {
			int result;
			if (TryCompare(value, valueToCompare, out result)) {
				return result;
			}

			return value.CompareTo(valueToCompare);
		}

		public static bool GetEqualsResult(IComparable value, IComparable valueToCompare) {
			int result;
			if (TryCompare(value, valueToCompare, out result)) {
				return result == 0;
			}

			return value.Equals(valueToCompare);
		}
	}
}