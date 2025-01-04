using BenchmarkDotNet.Attributes;
using FluentValidation.Validators;
using System;

namespace FluentValidation.Tests.Benchmarks;
public class PrecisionScale {

	private readonly PrecisionScaleValidator<Model> Validator = new(10, 3, true);
	private readonly Legacy.PrecisionScaleValidator<Model> LegacyValidator = new(10, 3, true);

	private readonly decimal[] Values = new decimal[1000];
	private readonly ValidationContext<Model> Context = new(new());

	public PrecisionScale()		{
		var rnd = new Random(42);
		for (var i = 0; i < Values.Length; i++) {
			Values[i] = 10_000_000m / (decimal)rnd.NextDouble();
			Values[i] /= (decimal)rnd.NextDouble();
			// 1 in 11 has (potentially) too much decimals.
			if(i % 11 != 0) {
				Values[i] -= Values[i] % 0.001m;
			}
			// 1 in 7 is negative.
			if(i % 7 == 1) {
				Values[i] *= -1;
			}
		}
	}

	[Benchmark(Baseline = true)]
	public bool Legacy() {
		var result = false;

		foreach (var value in Values) {
			result |= LegacyValidator.IsValid(Context, value);
		}

		return result;
	}

	[Benchmark]
	public bool Current() {
		var result = false;

		foreach (var value in Values) {
			result |= Validator.IsValid(Context, value);
		}

		return result;
	}


	private sealed class Model {
	}

}
