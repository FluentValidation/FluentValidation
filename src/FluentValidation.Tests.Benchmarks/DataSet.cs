#region License

// Copyright (c) .NET Foundation and contributors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// The latest version of this file can be found at https://github.com/FluentValidation/FluentValidation

#endregion

namespace FluentValidation.Tests.Benchmarks {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Bogus;
	using Bogus.Extensions;

	public static class DataSet {
		static DataSet() {
			void SetupFullModelManyErrorsFaker() {
				var nestedModelManyErrorsFaker = new Faker<NestedModel>()
					.RuleFor(m => m.Number1, m => m.Random.Int(0, 5))
					.RuleFor(m => m.Number2, m => m.Random.Int(0, 15))
					.RuleFor(m => m.Text1, m => m.Lorem.Word().OrNull(m, 0.1f))
					.RuleFor(m => m.Text2, m => m.Lorem.Word().OrNull(m, 0.15f))
					.RuleFor(m => m.SuperNumber1, m => m.Random.Number(0, 20).OrNull(m, 0.20f))
					.RuleFor(m => m.SuperNumber2, m => m.Random.Number(0, 20).OrNull(m, 0.25f));

				FullModelManyErrorsFaker = new Faker<FullModel>()
					.RuleFor(m => m.Text1, m => m.Lorem.Word().OrNull(m, 0.1f))
					.RuleFor(m => m.Text2, m => m.Lorem.Word().OrNull(m, 0.15f))
					.RuleFor(m => m.Text3, m => m.Lorem.Word().OrNull(m, 0.2f))
					.RuleFor(m => m.Text4, m => m.Lorem.Word().OrNull(m, 0.25f))
					.RuleFor(m => m.Text5, m => m.Lorem.Word().OrNull(m, 0.3f))
					.RuleFor(m => m.Number1, m => m.Random.Int(0, 5))
					.RuleFor(m => m.Number2, m => m.Random.Int(0, 10))
					.RuleFor(m => m.Number3, m => m.Random.Int(0, 15))
					.RuleFor(m => m.Number4, m => m.Random.Int(0, 20))
					.RuleFor(m => m.Number5, m => m.Random.Int(0, 25))
					.RuleFor(m => m.SuperNumber1, m => m.Random.Decimal(0, 20).OrNull(m, 0.20f))
					.RuleFor(m => m.SuperNumber2, m => m.Random.Decimal(0, 20).OrNull(m, 0.25f))
					.RuleFor(m => m.SuperNumber2, m => m.Random.Decimal(0, 20).OrNull(m, 0.30f))
					.RuleFor(m => m.NestedModel1, m => nestedModelManyErrorsFaker.Generate())
					.RuleFor(m => m.NestedModel2, m => nestedModelManyErrorsFaker.Generate())
					.RuleFor(m => m.ModelCollection, m => nestedModelManyErrorsFaker.GenerateBetween(0, 20).ToList().OrNull(m, 0.7f))
					.RuleFor(m => m.StructCollection, m => Enumerable.Range(1, m.Random.Int(1, 20)).Select(_ => m.Random.Number(0, 20)).ToList().OrNull(m, 0.7f));
			}

			void SetupFullModelNoErrorsFaker() {
				var nestedModelNoErrorsFaker = new Faker<NestedModel>()
					.RuleFor(m => m.Number1, m => m.Random.Int(0, 9))
					.RuleFor(m => m.Number2, m => m.Random.Int(0, 9))
					.RuleFor(m => m.Text1, m => m.Lorem.Word() + "a")
					.RuleFor(m => m.Text2, m => m.Lorem.Word() + "b")
					.RuleFor(m => m.SuperNumber1, m => m.Random.Number(0, 9))
					.RuleFor(m => m.SuperNumber2, m => m.Random.Number(0, 9));

				FullModelNoErrorsFaker = new Faker<FullModel>()
					.RuleFor(m => m.Text1, m => m.Lorem.Word() + "a")
					.RuleFor(m => m.Text2, m => m.Lorem.Word() + "b")
					.RuleFor(m => m.Text3, m => m.Lorem.Word() + "c")
					.RuleFor(m => m.Text4, m => m.Lorem.Word() + "d")
					.RuleFor(m => m.Text5, m => m.Lorem.Word() + "e")
					.RuleFor(m => m.Number1, m => m.Random.Int(0, 9))
					.RuleFor(m => m.Number2, m => m.Random.Int(0, 9))
					.RuleFor(m => m.Number3, m => m.Random.Int(0, 9))
					.RuleFor(m => m.Number4, m => m.Random.Int(0, 9))
					.RuleFor(m => m.Number5, m => m.Random.Int(0, 9))
					.RuleFor(m => m.SuperNumber1, m => m.Random.Decimal(0, 9))
					.RuleFor(m => m.SuperNumber2, m => m.Random.Decimal(0, 9))
					.RuleFor(m => m.SuperNumber3, m => m.Random.Decimal(0, 9))
					.RuleFor(m => m.NestedModel1, m => nestedModelNoErrorsFaker.Generate())
					.RuleFor(m => m.NestedModel2, m => nestedModelNoErrorsFaker.Generate())
					.RuleFor(m => m.ModelCollection, m => nestedModelNoErrorsFaker.GenerateBetween(0, 9).ToList())
					.RuleFor(m => m.StructCollection, m => Enumerable.Range(1, m.Random.Int(1, 9)).Select(_ => m.Random.Number(0, 9)).ToList());
			}

			void SetupFullModelHalfErrorsFaker() {
				var nestedModelsHalfErrorsFaker = new Faker<NestedModel>()
					.RuleFor(m => m.Number1, m => m.Random.Int(0, 9))
					.RuleFor(m => m.Number2, m => m.Random.Int(0, 9))
					.RuleFor(m => m.Text1, m => string.Join(" ", m.Lorem.Words(20)))
					.RuleFor(m => m.Text2, m => string.Join("b", m.Lorem.Words(15)))
					.RuleFor(m => m.SuperNumber1, m => m.Random.Number(0, 10))
					.RuleFor(m => m.SuperNumber2, m => m.Random.Number(0, 9).OrNull(m, 0.01f));

				FullModelHalfErrorsFaker = new Faker<FullModel>()
					.RuleFor(m => m.Text1, m => string.Join("a", m.Lorem.Words(10)))
					.RuleFor(m => m.Text2, m => string.Join("b", m.Lorem.Words(10)))
					.RuleFor(m => m.Text3, m => string.Join("c", m.Lorem.Words(10)))
					.RuleFor(m => m.Text4, m => string.Join("d", m.Lorem.Words(10)))
					.RuleFor(m => m.Text5, m => string.Join(" ", m.Lorem.Words(20)))
					.RuleFor(m => m.Number1, m => m.Random.Int(0, 9))
					.RuleFor(m => m.Number2, m => m.Random.Int(0, 9))
					.RuleFor(m => m.Number3, m => m.Random.Int(0, 9))
					.RuleFor(m => m.Number4, m => m.Random.Int(0, 9))
					.RuleFor(m => m.Number5, m => m.Random.Int(0, 10))
					.RuleFor(m => m.SuperNumber1, m => m.Random.Decimal(0, 9))
					.RuleFor(m => m.SuperNumber2, m => m.Random.Decimal(0, 9))
					.RuleFor(m => m.SuperNumber3, m => m.Random.Decimal(0, 10).OrNull(m, 0.01f))
					.RuleFor(m => m.NestedModel1, m => nestedModelsHalfErrorsFaker.Generate())
					.RuleFor(m => m.NestedModel2, m => nestedModelsHalfErrorsFaker.Generate())
					.RuleFor(m => m.ModelCollection, m => nestedModelsHalfErrorsFaker.GenerateBetween(0, 9).ToList())
					.RuleFor(m => m.StructCollection, m => Enumerable.Range(1, m.Random.Int(1, 11)).Select(_ => m.Random.Number(0, 9)).ToList());
			}

			SetupFullModelHalfErrorsFaker();
			SetupFullModelManyErrorsFaker();
			SetupFullModelNoErrorsFaker();

			Size = 10_000;

			Randomizer.Seed = new Random(666);

			ManyErrorsDataSet = FullModelManyErrorsFaker.GenerateLazy(Size).ToList();
			HalfErrorsDataSet = FullModelHalfErrorsFaker.GenerateLazy(Size).ToList();
			NoErrorsDataSet = FullModelNoErrorsFaker.GenerateLazy(Size).ToList();

			DataSets = new Dictionary<string, IReadOnlyList<FullModel>>(3) {
				["ManyErrors"] = ManyErrorsDataSet,
				["HalfErrors"] = HalfErrorsDataSet,
				["NoErrors"] = NoErrorsDataSet
			};
		}

		public static int Size { get; }

		public static IReadOnlyList<FullModel> ManyErrorsDataSet { get; }

		public static IReadOnlyList<FullModel> HalfErrorsDataSet { get; }

		public static IReadOnlyList<FullModel> NoErrorsDataSet { get; }

		public static IReadOnlyDictionary<string, IReadOnlyList<FullModel>> DataSets { get; }

		public static Faker<FullModel> FullModelManyErrorsFaker { get; private set; }

		public static Faker<FullModel> FullModelNoErrorsFaker { get; private set; }

		public static Faker<FullModel> FullModelHalfErrorsFaker { get; private set; }
	}
}
