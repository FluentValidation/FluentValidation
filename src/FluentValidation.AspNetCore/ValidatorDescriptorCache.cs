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

namespace FluentValidation.AspNetCore {
	using System;
	using System.Collections.Generic;
	using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

	/// <summary>
	/// Caches the validators used when generating clientside metadata.
	/// Ideally, the validators wouldbe singletons so this happens automatically,
	/// but we can't rely on this behaviour. The user may have registered them as something else
	/// And as of 10.0, the default is to auto-register validators as scoped as inexperienced developers
	/// often have issues understanding issues that arise from having singleton-scoped objects depending on non-singleton-scoped services
	/// Instead, we can cache the validators used for clientside validation in Httpcontext.Items to prevent them being instantiated once per property.
	/// </summary>
	internal class ValidatorDescriptorCache {
		private const string CacheKey = "_FV_ClientValidation_Cache";

		public IValidatorDescriptor GetCachedDescriptor(ClientValidatorProviderContext context, IHttpContextAccessor httpContextAccessor) {
			if (httpContextAccessor == null) {
				throw new InvalidOperationException("Cannot use clientside validation unless the IHttpContextAccessor is registered with the service provider. Make sure the provider is registered by calling services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); in your Startup class's ConfigureServices method");
			}

			var modelType = context.ModelMetadata.ContainerType;
			if (modelType == null) return null;

			Dictionary<Type, IValidatorDescriptor> cache = GetCache(httpContextAccessor.HttpContext.Items);

			if (cache.TryGetValue(modelType, out var descriptor)) {
				return descriptor;
			}

#pragma warning disable CS0618
			var validatorFactory = (IValidatorFactory)httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IValidatorFactory));
#pragma warning restore CS0618

			var validator = validatorFactory.GetValidator(modelType);
			descriptor = validator?.CreateDescriptor();
			cache[modelType] = descriptor;
			return descriptor;
		}

		private Dictionary<Type, IValidatorDescriptor> GetCache(IDictionary<object, object> httpContextItems) {
			Dictionary<Type, IValidatorDescriptor> cache = null;

			if(httpContextItems.ContainsKey(CacheKey)) {
				cache = httpContextItems[CacheKey] as Dictionary<Type, IValidatorDescriptor>;
			}

			if (cache == null) {
				cache = new Dictionary<Type, IValidatorDescriptor>();
				httpContextItems[CacheKey] = cache;
			}

			return cache;
		}
	}
}
