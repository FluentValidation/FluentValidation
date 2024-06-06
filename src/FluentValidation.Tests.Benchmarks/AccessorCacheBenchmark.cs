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
	using System.Linq.Expressions;
	using System.Reflection;
	using BenchmarkDotNet.Attributes;
	using Internal;

	[MemoryDiagnoser]
	public class AccessorCacheBenchmark {

		private Expression<Func<TestModel, int>> Expression { get; set; }
		private MemberInfo Member { get; set; }

		[GlobalSetup]
		public void GlobalSetup() {
			Expression = GetExpression<TestModel, int>(x => x.Property);
			Member = Expression.GetMember();
		}

		[Benchmark]
		public Func<TestModel, int> GetCachedAccessor() {
			return AccessorCache<TestModel>.GetCachedAccessor(Member, Expression, false, "");
		}

		[Benchmark]
		public Func<TestModel, int> GetCachedAccessorWithCachePrefix() {
			return AccessorCache<TestModel>.GetCachedAccessor(Member, Expression, false, "Prefix");
		}

		private Expression<Func<T, TProperty>> GetExpression<T, TProperty>(Expression<Func<T, TProperty>> expression) {
			return expression;
		}

		public class TestModel {
			public int Property { get; set; }
		}
	}
}
