using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DotNetLiberty.AspNet.Testing.Samples
{
    /// <summary>
    /// This is your application startup class. 
    /// 
    /// You have a few options:
    ///
    /// (a) You can add a reference to your web application project and use the startup class directly.
    ///
    /// (b) Alternatively, you can inherit from your real startup class and override any changes you need to make,
    ///     such as injecting fake services.
    ///
    /// (c) Use an entirely new Startup that you define just for service testing.
    /// </summary>
    public class MinimalApplicationStartup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }

    /// <summary>
    /// This test will fire up a new instance of your web application for each test method.
    ///
    /// We implement <see cref="IDisposable"/> so that we can cleanly shut down the server after each test.
    /// </summary>
    public class NewServerEachTest : IDisposable
    {
        private readonly WebApplicationTestHarness _webAppTestHarness;
        private const string WebAppUrl = "http://localhost:5151";

        /// <summary>
        /// We create a new test harness in the constructor - this runs afresh for each test method.
        /// </summary>
        public NewServerEachTest()
        {
            // Initialize the test web application with standard switches
            _webAppTestHarness = WebApplicationTest.Run<MinimalApplicationStartup>(new[]
            {
                "--server",
                "Microsoft.AspNet.Server.Kestrel",
                "--server.urls",
                WebAppUrl
            });
        }

        /// <summary>
        /// We have to call dispose on the test harness to shut down the web application cleanly.
        /// </summary>
        public void Dispose()
        {
            _webAppTestHarness.Dispose();
        }

        /// <summary>
        /// By the time this test runs, a fresh copy of the web application has been started up
        /// </summary>
        [Fact]
        public async void can_get_hello_world()
        {
            // Arrange
            var client = new HttpClient();

            // Act
            var result = await client.GetStringAsync(WebAppUrl);

            // Assert
            Assert.Equal("Hello World!", result);
        }
    }
}
