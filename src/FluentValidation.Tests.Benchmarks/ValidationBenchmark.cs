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
	using System.Collections.Generic;
	using BenchmarkDotNet.Attributes;

	[MemoryDiagnoser]
	public class ValidationBenchmark {
		private FullModelValidator _validator;
		private FullModelValidator _failFastValidator;

		private IReadOnlyDictionary<string, IReadOnlyList<FullModel>> _dataSets;

		[Params("ManyErrors", "HalfErrors", "NoErrors")]
		public string DataSet { get; set; }

		[GlobalSetup]
		public void GlobalSetup() {
			_validator = new FullModelValidator();
			_failFastValidator = new FullModelValidator {CascadeMode = CascadeMode.Stop};
			_dataSets = FluentValidation.Tests.Benchmarks.DataSet.DataSets;
		}

		[Benchmark]
		public object FailFast() {
			var models = _dataSets[DataSet];

			object t = null;

			for (var i = 0; i < models.Count; ++i) {
				t = _failFastValidator.Validate(models[i]);
			}

			return t;
		}

		[Benchmark]
		public object Validate() {
			var models = _dataSets[DataSet];

			var t = new object();

			for (var i = 0; i < models.Count; ++i) {
				t = _validator.Validate(models[i]);
			}

			return t;
		}
	}
}
