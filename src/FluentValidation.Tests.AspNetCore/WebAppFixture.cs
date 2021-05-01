namespace FluentValidation.Tests {
	using System;
	using System.Net.Http;
	using AspNetCore;
	using FluentValidation.AspNetCore;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc.Testing;
	using Microsoft.Extensions.DependencyInjection;

	public class WebAppFixture : WebApplicationFactory<Startup> {
		protected override void ConfigureWebHost(IWebHostBuilder builder) {
			builder.UseContentRoot(".");
		}

		protected override IWebHostBuilder CreateWebHostBuilder() {
			return new WebHostBuilder()
				.UseDefaultServiceProvider((context, options) => options.ValidateScopes = true)
				.UseStartup<Startup>();
		}

		public HttpClient CreateClientWithServices(Action<IServiceCollection> configurator) {
			return WithWebHostBuilder(builder => builder.ConfigureServices(configurator)).CreateClient();
		}
	}
}
