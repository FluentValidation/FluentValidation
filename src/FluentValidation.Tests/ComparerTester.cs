namespace FluentValidation.Tests {
	using System;
	using Internal;
	using Xunit;

	
	public class ComparerTester {
		[Fact]
		public void Should_fail_with_different_type_values() {
			double myDouble = 100.12;
			long myLong = 100;

			Comparer.GetEqualsResult(myLong, myDouble).ShouldBeFalse();
			Comparer.GetEqualsResult(myDouble, myLong).ShouldBeFalse();
			Comparer.GetComparisonResult(myDouble, myLong).ShouldEqual(1);
			Comparer.GetComparisonResult(myLong, myDouble).ShouldEqual(-1);

			int result;
			Comparer.TryCompare(myDouble, myLong, out result).ShouldBeTrue();
			result.ShouldEqual(1);
			Comparer.TryCompare(myLong, myDouble, out result).ShouldBeTrue();
			result.ShouldEqual(-1);
		}

		[Fact]
		public void Should_succeed_with_same_type_values() {
			double myDouble = 100.12;
			double myOther = 100.12;

			Comparer.GetEqualsResult(myOther, myDouble).ShouldBeTrue();
			Comparer.GetEqualsResult(myDouble, myOther).ShouldBeTrue();
			Comparer.GetComparisonResult(myOther, myDouble).ShouldEqual(0);
			Comparer.GetComparisonResult(myDouble, myOther).ShouldEqual(0);

			int result;
			Comparer.TryCompare(myDouble, myOther, out result).ShouldBeTrue();
			result.ShouldEqual(0);
			Comparer.TryCompare(myOther, myDouble, out result).ShouldBeTrue();
			result.ShouldEqual(0);
		}

		[Fact]
		public void Should_succeed_with_same_object_values() {
			var first = new MyObject {Id = 5};
			var second = new MyObject {Id = 5};

			Comparer.GetEqualsResult(first, second).ShouldBeTrue();
			Comparer.GetEqualsResult(second, first).ShouldBeTrue();
			Comparer.GetComparisonResult(first, second).ShouldEqual(0);
			Comparer.GetComparisonResult(second, first).ShouldEqual(0);

			int result;
			Comparer.TryCompare(first, second, out result).ShouldBeTrue();
			result.ShouldEqual(0);
			Comparer.TryCompare(second, first, out result).ShouldBeTrue();
			result.ShouldEqual(0);
		}

		[Fact]
		public void Should_fail_with_different_object_values() {
			var first = new MyObject {Id = 5};
			var second = new MyObject {Id = 6};

			Comparer.GetEqualsResult(first, second).ShouldBeFalse();
			Comparer.GetEqualsResult(second, first).ShouldBeFalse();
			Comparer.GetComparisonResult(first, second).ShouldEqual(-1);
			Comparer.GetComparisonResult(second, first).ShouldEqual(1);

			int result;
			Comparer.TryCompare(first, second, out result).ShouldBeTrue();
			result.ShouldEqual(-1);
			Comparer.TryCompare(second, first, out result).ShouldBeTrue();
			result.ShouldEqual(1);
		}

		[Fact]
		public void Should_fail_with_different_object_types() {
			var first = new MyObject {Id = 5};
			var second = 5;

			Comparer.GetEqualsResult(first, second).ShouldBeFalse();
			Comparer.GetEqualsResult(second, first).ShouldBeFalse();

			try {
				Comparer.GetComparisonResult(first, second);
				Assert.True(false, "Should never get here!");
			}
			catch (ArgumentException ex) {
				ex.Message.ShouldEqual("Cannot compare MyObject with anything other than another MyObject.");
			}

			try {
				Comparer.GetComparisonResult(second, first);
				Assert.True(false, "Should never get here!");
			}
			catch (ArgumentException ex) {
				ex.Message.ShouldEqual("Object must be of type Int32.");
			}

			int result;
			Comparer.TryCompare(first, second, out result).ShouldBeFalse();
			result.ShouldEqual(0);
			Comparer.TryCompare(second, first, out result).ShouldBeFalse();
			result.ShouldEqual(0);
		}

		public class MyObject : IComparable<MyObject>, IComparable {
			public int Id { get; set; }

			public int CompareTo(MyObject other) {
				return Id.CompareTo(other.Id);
			}

			int IComparable.CompareTo(object obj) {
				var o = obj as MyObject;
				if (o != null) {
					return CompareTo(o);
				}
				throw new ArgumentException("Cannot compare MyObject with anything other than another MyObject.");
			}
		}
	}
}
