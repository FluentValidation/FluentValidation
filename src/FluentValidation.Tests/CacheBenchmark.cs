namespace FluentValidation.Tests {
	using System;
	using System.Diagnostics;
	using System.Linq.Expressions;
	using Internal;
	using Xunit;
	using Xunit.Abstractions;

	public class CacheBenchmark {
		private readonly ITestOutputHelper output;

		public CacheBenchmark(ITestOutputHelper output) {
			this.output = output;
		}

		[Fact]
		public void Equality_comparison_check() {
			Expression<Func<Person, string>> expr1 = x => x.Surname;
			Expression<Func<Person, string>> expr2 = x => x.Surname;
			Expression<Func<Person, string>> expr3 = x => x.Forename;

			var member1 = expr1.GetMember();
			var member2 = expr2.GetMember();
			var member3 = expr3.GetMember();

			Assert.Equal(member1, member2);
			Assert.NotEqual(member1, member3);
		}

		[Fact(Skip = "Manual benchmark")]
		public void Bemchmark() {
			var s = new Stopwatch();
			s.Start();

			for(int i = 0; i < 20000; i++)
			{
				var v = new BenchmarkValidator();
			}

			s.Stop();
			output.WriteLine(s.Elapsed.ToString());
		}

		private class BenchmarkValidator : AbstractValidator<Person> {
			public BenchmarkValidator() {
				RuleFor(x => x.Surname).NotNull();
				RuleFor(x => x).Must(x => true);
			}
		}
	}
}