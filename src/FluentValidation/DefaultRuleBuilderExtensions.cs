#region License
// Copyright 2008-2009 Jeremy Skinner (http://www.jeremyskinner.co.uk)
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
// The latest version of this file can be found at http://www.codeplex.com/FluentValidation
#endregion

namespace FluentValidation {
	using Internal;

	/// <summary>
	/// Default extensions for IRuleBuilder
	/// </summary>
	public static class DefaultRuleBuilderExtensions {
		public static IFluentInterfaceMarker<CascadeMode, IRuleBuilderInitial<T, TProperty>> Cascade<T, TProperty>(this IRuleBuilderInitial<T, TProperty> ruleBuilder) {
			var marker = new FluentInterfaceMarker<CascadeMode, IRuleBuilderInitial<T,TProperty>>(ruleBuilder);
			return marker;
		}

		public static IRuleBuilderInitial<T,TProperty> StopOnFirstFailure<T,TProperty>(this IFluentInterfaceMarker<CascadeMode,IRuleBuilderInitial<T, TProperty>> marker) {
			return marker.ToMarker().Next.Configure(rb => {
				rb.CascadeMode = CascadeMode.StopOnFirstFailure;
			});
		}

		public static IRuleBuilderInitial<T, TProperty> Continue<T, TProperty>(this IFluentInterfaceMarker<CascadeMode, IRuleBuilderInitial<T, TProperty>> marker) {
			return marker.ToMarker().Next.Configure(rb => {
				rb.CascadeMode = CascadeMode.Continue;
			});
		}

		private static FluentInterfaceMarker<T, TNext> ToMarker<T,TNext>(this IFluentInterfaceMarker<T,TNext> marker) {
			return (FluentInterfaceMarker<T, TNext>)marker;
		}
	}
}