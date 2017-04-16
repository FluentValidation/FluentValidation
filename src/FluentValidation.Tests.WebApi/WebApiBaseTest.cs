#region License
// Copyright (c) Jeremy Skinner (http://www.jeremyskinner.co.uk)
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
// The latest version of this file can be found at https://github.com/JeremySkinner/FluentValidation
#endregion
namespace FluentValidation.Tests.WebApi {
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Net.Http;
	using System.Net.Http.Headers;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Web.Http;
	using FluentValidation.WebApi;

	public abstract class WebApiBaseTest {
		protected List<SimpleError> InvokeTest<T>(string input, string contentType = "application/x-www-form-urlencoded") {
			const string baseAddress = "http://dummyname/";

			string className = typeof(T).Name;

			// Server
			HttpConfiguration config = new HttpConfiguration();
			config.Routes.MapHttpRoute("Default", "api/{controller}/{action}/{id}", new {id = RouteParameter.Optional});
			config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
			FluentValidationModelValidatorProvider.Configure(config);

			HttpServer server = new HttpServer(config);

			// Client
			HttpMessageInvoker messageInvoker = new HttpMessageInvoker(new InMemoryHttpContentSerializationHandler(server));

			//order to be created
			//			Order requestOrder = new Order() { OrderId = "A101", OrderValue = 125.00, OrderedDate = DateTime.Now.ToUniversalTime(), ShippedDate = DateTime.Now.AddDays(2).ToUniversalTime() };

			HttpRequestMessage request = new HttpRequestMessage();
			request.Content = new StringContent(input, Encoding.UTF8, contentType); /* JsonContent(@"{
				SomeBool:'false',
				Id:0}");
*/
			request.RequestUri = new Uri(baseAddress + "api/Test/" + className);
			request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
			request.Method = HttpMethod.Post;

			CancellationTokenSource cts = new CancellationTokenSource();

			using (HttpResponseMessage response = messageInvoker.SendAsync(request, cts.Token).Result) {
				var errors = response.Content.ReadAsAsync<List<SimpleError>>().Result;
				return errors;
			}
		}

		class InMemoryHttpContentSerializationHandler : DelegatingHandler {
			public InMemoryHttpContentSerializationHandler() {
			}

			public InMemoryHttpContentSerializationHandler(HttpMessageHandler innerHandler)
				: base(innerHandler) {
			}

			protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
				// Replace the original content with a StreamContent before the request
				// passes through upper layers in the stack
				request.Content = ConvertToStreamContent(request.Content);

				return base.SendAsync(request, cancellationToken).ContinueWith<HttpResponseMessage>((responseTask) => {
					HttpResponseMessage response = responseTask.Result;

					// Replace the original content with a StreamContent before the response
					// passes through lower layers in the stack
					response.Content = ConvertToStreamContent(response.Content);

					return response;
				});
			}

			StreamContent ConvertToStreamContent(HttpContent originalContent) {
				if (originalContent == null) {
					return null;
				}

				StreamContent streamContent = originalContent as StreamContent;

				if (streamContent != null) {
					return streamContent;
				}

				MemoryStream ms = new MemoryStream();

				// **** NOTE: ideally you should NOT be doing calling Wait() as its going to block this thread ****
				// if the original content is an ObjectContent, then this particular CopyToAsync() call would cause the MediaTypeFormatters to 
				// take part in Serialization of the ObjectContent and the result of this serialization is stored in the provided target memory stream.
				originalContent.CopyToAsync(ms).Wait();

				// Reset the stream position back to 0 as in the previous CopyToAsync() call,
				// a formatter for example, could have made the position to be at the end after serialization
				ms.Position = 0;

				streamContent = new StreamContent(ms);

				// copy headers from the original content
				foreach (KeyValuePair<string, IEnumerable<string>> header in originalContent.Headers) {
					streamContent.Headers.TryAddWithoutValidation(header.Key, header.Value);
				}

				return streamContent;
			}
		}


	}
}