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
	using System.Linq;
	using BenchmarkDotNet.Attributes;

	[MemoryDiagnoser]
	public class EngineOnlyBenchmark {
		private IReadOnlyList<VoidModel> _noLogicModels;

		private NoLogicModelSingleRuleValidator _fluentValidationSingleRuleValidator;

		private NoLogicModelTenRulesValidator _fluentValidationTenRulesValidator;

		public class VoidModel {
			public object Member { get; set; }
		}

		public class NoLogicModelSingleRuleValidator : AbstractValidator<VoidModel> {
			public NoLogicModelSingleRuleValidator() {
				RuleFor(m => m.Member).Must(o => true);
			}
		}

		public class NoLogicModelTenRulesValidator : AbstractValidator<VoidModel> {
			public NoLogicModelTenRulesValidator() {
				RuleFor(m => m.Member).Must(o => true);
				RuleFor(m => m.Member).Must(o => true);
				RuleFor(m => m.Member).Must(o => true);
				RuleFor(m => m.Member).Must(o => true);
				RuleFor(m => m.Member).Must(o => true);
				RuleFor(m => m.Member).Must(o => true);
				RuleFor(m => m.Member).Must(o => true);
				RuleFor(m => m.Member).Must(o => true);
				RuleFor(m => m.Member).Must(o => true);
				RuleFor(m => m.Member).Must(o => true);
			}
		}

		[Params(10000)]
		public int N { get; set; }

		[GlobalSetup]
		public void GlobalSetup() {
			_fluentValidationSingleRuleValidator = new NoLogicModelSingleRuleValidator();
			_fluentValidationTenRulesValidator = new NoLogicModelTenRulesValidator();
			_noLogicModels = Enumerable.Range(0, N).Select(m => new VoidModel() {Member = new object()}).ToList();
		}

		[Benchmark]
		public object Validate_SingleRule() {
			object t = null;

			for (var i = 0; i < N; ++i) {
				t = _fluentValidationSingleRuleValidator.Validate(_noLogicModels[i]);
			}

			return t;
		}

		[Benchmark]
		public object Validate_TenRules() {
			object t = null;

			for (var i = 0; i < N; ++i) {
				t = _fluentValidationTenRulesValidator.Validate(_noLogicModels[i]);
			}

			return t;
		}
	}
}
