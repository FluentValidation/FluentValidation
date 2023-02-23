namespace FluentValidation.Tests;

#nullable enable
using System;
using System.Collections.Generic;

public class NullablePlayground {

	public int Int { get; set; }
	public int? NullableInt { get; set; }
	public string Str { get; set; } = string.Empty;
	public string? NullableStr { get; set; }

	public List<NullablePlayground> Children { get; set; } = new();

	public List<NullablePlayground>? Children2 { get; set; } = new();

	public List<NullablePlayground?> Children3 { get; set; } = new();

	public List<NullablePlayground?>? Children4 { get; set; } = new();
	public int[] Ints { get; set; } = Array.Empty<int>();

	public int?[] NullableInts { get; set; } = Array.Empty<int?>();

	public int?[]? NullableIntsNullable { get; set; }

	public string[] Strings { get; set; } = Array.Empty<string>();

	public string?[] NullableStrings { get; set; } = Array.Empty<string?>();

	public string?[]? NullableStringsNullable { get; set; }


	class Validator : AbstractValidator<NullablePlayground> {

		public Validator() {
			// Should all compile.
			RuleFor(x => x.Int).NotNull().WithMessage("foo");
			RuleFor(x => x.NullableInt).NotNull().WithMessage("foo");
			RuleFor(x => x.Str).NotNull().WithMessage("foo");
			RuleFor(x => x.NullableStr).NotNull().WithMessage("foo");

			RuleForEach(x => x.Children).NotNull().WithMessage("foo");
			RuleForEach(x => x.Children2).NotNull().WithMessage("foo");
			RuleForEach(x => x.Children3).NotNull().WithMessage("foo");
			RuleForEach(x => x.Children4).NotNull().WithMessage("foo");

			RuleForEach(x => x.Ints).NotNull().WithMessage("foo");
			RuleForEach(x => x.NullableInts).NotNull().WithMessage("foo");
			RuleForEach(x => x.NullableIntsNullable).NotNull().WithMessage("foo");
			RuleForEach(x => x.Strings).NotNull().WithMessage("foo");
			RuleForEach(x => x.NullableStrings).NotNull().WithMessage("foo");
			RuleForEach(x => x.NullableStringsNullable).NotNull().WithMessage("foo");

			RuleFor(x => x.Str).Length(1, 10);
			RuleFor(x => x.NullableStr).Length(1, 10);

			RuleFor(x => x.Str).Matches("foo");
			RuleFor(x => x.NullableStr).Matches("foo");

			RuleFor(x => x.Str).MaximumLength(5);
			RuleFor(x => x.NullableStr).MaximumLength(5);

			RuleFor(x => x.Str).MinimumLength(5);
			RuleFor(x => x.NullableStr).MinimumLength(5);

			RuleFor(x => x.Str).EmailAddress();
			RuleFor(x => x.NullableStr).EmailAddress();

			RuleFor(x => x.Str).NotEqual("foo");
			RuleFor(x => x.NullableStr).NotEqual("foo");
			RuleFor(x => x.Str).NotEqual((string?)"foo");
			RuleFor(x => x.NullableStr).NotEqual((string?)"foo");

			RuleFor(x => x.Str).NotEqual("foo", StringComparer.Ordinal);
			RuleFor(x => x.NullableStr).NotEqual("foo", StringComparer.Ordinal);
			RuleFor(x => x.Str).NotEqual((string?)"foo", StringComparer.Ordinal);
			RuleFor(x => x.NullableStr).NotEqual((string?)"foo", StringComparer.Ordinal);

			RuleFor(x => x.Str).NotEqual(x => x.Str);
			RuleFor(x => x.NullableStr).NotEqual(x => x.Str);
			RuleFor(x => x.Str).NotEqual(x => x.NullableStr);
			RuleFor(x => x.NullableStr).NotEqual(x => x.NullableStr);

			RuleFor(x => x.Int).NotEqual(0);
			RuleFor(x => x.NullableInt).NotEqual((int?)null);
			RuleFor(x => x.Int).NotEqual(0);
			RuleFor(x => x.NullableInt).NotEqual((int?)null);

			RuleFor(x => x.Int).NotEqual(x => x.Int);
			RuleFor(x => x.NullableInt).NotEqual(x => x.Int);
			RuleFor(x => x.NullableInt).NotEqual(x => x.NullableInt);

			RuleFor(x => x.Str).Equal("foo");
			RuleFor(x => x.NullableStr).Equal("foo");
			RuleFor(x => x.Str).Equal((string?)"foo");
			RuleFor(x => x.NullableStr).Equal((string?)"foo");

			RuleFor(x => x.Str).Equal("foo", StringComparer.Ordinal);
			RuleFor(x => x.NullableStr).Equal("foo", StringComparer.Ordinal);
			RuleFor(x => x.Str).Equal((string?)"foo", StringComparer.Ordinal);
			RuleFor(x => x.NullableStr).Equal((string?)"foo", StringComparer.Ordinal);

			RuleFor(x => x.Str).Equal(x => x.Str);
			RuleFor(x => x.NullableStr).Equal(x => x.Str);
			RuleFor(x => x.Str).Equal(x => x.NullableStr);
			RuleFor(x => x.NullableStr).Equal(x => x.NullableStr);

			RuleFor(x => x.Int).Equal(0);
			RuleFor(x => x.NullableInt).Equal((int?)null);
			RuleFor(x => x.Int).Equal(0);
			RuleFor(x => x.NullableInt).Equal((int?)null);

			RuleFor(x => x.Int).Equal(x => x.Int);
			RuleFor(x => x.NullableInt).Equal(x => x.Int);
			RuleFor(x => x.NullableInt).Equal(x => x.NullableInt);

			RuleFor(x => x.Int).LessThan(5);
			RuleFor(x => x.NullableInt).LessThan(5);
			RuleFor(x => x.Int).LessThan(x => x.Int);
			RuleFor(x => x.NullableInt).LessThan(x => x.Int);
			RuleFor(x => x.Int).LessThan(x => x.NullableInt);
			RuleFor(x => x.NullableInt).LessThan(x => x.NullableInt);

			string s1 = null!;
			string? s2 = null;

			IComparable<string> c1 = s1;
			IComparable<string?> c2 = s1;

			IComparable<string> c3 = s2!;
			IComparable<string?> c4 = s2!;


			RuleFor(x => x.Str).LessThan("foo");
			RuleFor(x => x.NullableStr).LessThan("foo");
			RuleFor(x => x.Str).LessThan(x => x.Str);
			RuleFor(x => x.NullableStr).LessThan(x => x.Str);
			RuleFor(x => x.Str).LessThan(x => x.NullableStr);
			RuleFor(x => x.NullableStr).LessThan(x => x.NullableStr);

			RuleFor(x => x.Int).GreaterThan(5);
			RuleFor(x => x.NullableInt).GreaterThan(5);
			RuleFor(x => x.Int).GreaterThan(x => x.Int);
			RuleFor(x => x.NullableInt).GreaterThan(x => x.Int);
			RuleFor(x => x.Int).GreaterThan(x => x.NullableInt);
			RuleFor(x => x.NullableInt).GreaterThan(x => x.NullableInt);

			RuleFor(x => x.Str).GreaterThan("foo");
			RuleFor(x => x.NullableStr).GreaterThan("foo");
			RuleFor(x => x.Str).GreaterThan(x => x.Str);
			RuleFor(x => x.NullableStr).GreaterThan(x => x.Str);
			RuleFor(x => x.Str).GreaterThan(x => x.NullableStr);
			RuleFor(x => x.NullableStr).GreaterThan(x => x.NullableStr);

			RuleFor(x => x.Int).LessThanOrEqualTo(5);
			RuleFor(x => x.NullableInt).LessThanOrEqualTo(5);
			RuleFor(x => x.Int).LessThanOrEqualTo(x => x.Int);
			RuleFor(x => x.NullableInt).LessThanOrEqualTo(x => x.Int);
			RuleFor(x => x.Int).LessThanOrEqualTo(x => x.NullableInt);
			RuleFor(x => x.NullableInt).LessThanOrEqualTo(x => x.NullableInt);

			RuleFor(x => x.Str).LessThanOrEqualTo("foo");
			RuleFor(x => x.NullableStr).LessThanOrEqualTo("foo");
			RuleFor(x => x.Str).LessThanOrEqualTo(x => x.Str);
			RuleFor(x => x.NullableStr).LessThanOrEqualTo(x => x.Str);
			RuleFor(x => x.Str).LessThanOrEqualTo(x => x.NullableStr);
			RuleFor(x => x.NullableStr).LessThanOrEqualTo(x => x.NullableStr);

			RuleFor(x => x.Int).GreaterThanOrEqualTo(5);
			RuleFor(x => x.NullableInt).GreaterThanOrEqualTo(5);
			RuleFor(x => x.Int).GreaterThanOrEqualTo(x => x.Int);
			RuleFor(x => x.NullableInt).GreaterThanOrEqualTo(x => x.Int);
			RuleFor(x => x.Int).GreaterThanOrEqualTo(x => x.NullableInt);
			RuleFor(x => x.NullableInt).GreaterThanOrEqualTo(x => x.NullableInt);

			RuleFor(x => x.Str).LessThanOrEqualTo("foo");
			RuleFor(x => x.NullableStr).LessThanOrEqualTo("foo");
			RuleFor(x => x.Str).LessThanOrEqualTo(x => x.Str);
			RuleFor(x => x.NullableStr).LessThanOrEqualTo(x => x.Str);
			RuleFor(x => x.Str).LessThanOrEqualTo(x => x.NullableStr);
			RuleFor(x => x.NullableStr).LessThanOrEqualTo(x => x.NullableStr);

			RuleFor(x => x.Int).InclusiveBetween(1, 10);
			RuleFor(x => x.NullableInt).InclusiveBetween(1, 10);
			RuleFor(x => x.Str).InclusiveBetween("aa", "zz");
			RuleFor(x => x.NullableStr).InclusiveBetween("aa", "zz");
			RuleFor(x => x.Str).InclusiveBetween("aa", "zz", StringComparer.Ordinal);
			RuleFor(x => x.NullableStr).InclusiveBetween("aa", "zz", StringComparer.Ordinal);

			RuleFor(x => x.Int).ExclusiveBetween(1, 10);
			RuleFor(x => x.NullableInt).ExclusiveBetween(1, 10);
			RuleFor(x => x.Str).ExclusiveBetween("aa", "zz");
			RuleFor(x => x.NullableStr).ExclusiveBetween("aa", "zz");
			RuleFor(x => x.Str).ExclusiveBetween("aa", "zz", StringComparer.Ordinal);
			RuleFor(x => x.NullableStr).ExclusiveBetween("aa", "zz", StringComparer.Ordinal);


			RuleFor(x => x.Children).ForEach(c => {
				c.NotNull();
			});
			RuleFor(x => x.Children2).ForEach(c => {
				c.NotNull();
			});
			RuleFor(x => x.Children3).ForEach(c => {
				c.NotNull();
			});
			RuleFor(x => x.Children4).ForEach(c => {
				c.NotNull();
			});
		}
	}

}
