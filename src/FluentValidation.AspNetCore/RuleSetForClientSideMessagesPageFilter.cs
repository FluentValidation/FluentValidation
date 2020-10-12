#if NETCOREAPP3_1 || NET5_0

#region License
// Copyright (c) .NET Foundation and contributors
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

namespace FluentValidation.AspNetCore {
	using System.Linq;
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Mvc.Filters;

	/// <summary>
	/// Determines which ruleset should be used when deciding which validators should be used to generate client-side messages.
	/// </summary>
	internal class RuleSetForClientSideMessagesPageFilter : IAsyncPageFilter {

		public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context) {
			return Task.CompletedTask;
		}

		public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next) {
			var attribute = context.HandlerMethod?.MethodInfo?.GetCustomAttributes(typeof(RuleSetForClientSideMessagesAttribute), true).FirstOrDefault();

			if(attribute is RuleSetForClientSideMessagesAttribute ruleSetAttribute) {
				ruleSetAttribute.SetRulesetForClientValidation(context.HttpContext);
			}

			await next.Invoke();
		}
	}
}

#endif
