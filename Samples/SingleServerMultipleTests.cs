using System;
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
    public class SingleServerStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app)
        {
            int times = 0;
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync($"I have been invoked {++times} times");
            });
        }
    }

    /// <summary>
    /// We have to create a "test fixture" which is a class that is responsible for
    /// creating and destroying a resource that is shared amongst multiple tests - in this case our test web application.
    ///
    /// Rather than implement our own fixture that calls into <see cref="WebApplicationTest"/>, I'm extending
    /// the <see cref="WebApplicationTestFixture"/> which does the call for us.
    ///
    /// Details about fixtures here: http://xunit.github.io/docs/shared-context.html#class-fixture
    /// </summary>
    public class SingleServerTestFixture : WebApplicationTestFixture<SingleServerStartup>
    {
        public SingleServerTestFixture()
            : base(new []
            {
                "--server",
                "Microsoft.AspNet.Server.Kestrel",
                "--server.urls",
                SingleServerMultipleTests.WebAppUrl
            })
        { }

        /// <summary>
        /// For the sake of this demo, I'm going to keep track of how many tests have run.
        /// The web server is also keeping track. This is so we can prove that the same server
        /// is in fact reused for multiple tests.
        /// </summary>
        public int TestExecutedCount { get; set; }
        
    }

    /// <summary>
    /// Since this test class is decorated with a <see cref="IClassFixture{TFixture}"/> interface, xUnit
    /// will create the test fixture for us before the tests in this class run.
    /// 
    /// </summary>
    public class SingleServerMultipleTests : IClassFixture<SingleServerTestFixture>
    {
        private readonly SingleServerTestFixture _testFixture;
        // I'm choosing a different port in each class since xUnit may run other test classes at the same time
        internal const string WebAppUrl = "http://localhost:5252";

        /// <summary>
        /// If desired, you can add a constructor that takes in a <see cref="SingleServerTestFixture"/> if you
        /// want a reference to this fixture. This is 100% optional.
        ///
        /// I need a reference so I can keep track of how many tests have ran.
        /// </summary>
        public SingleServerMultipleTests(SingleServerTestFixture testFixture)
        {
            _testFixture = testFixture;
        }

        /// <summary>
        /// By the time this test runs, a fresh shared copy of the web application has been started up.
        ///
        /// This test may run first or second.
        /// </summary>
        [Fact]
        public async void one_test_to_get()
        {
            // Arrange
            var client = new HttpClient();
            var testIndex = ++_testFixture.TestExecutedCount;
            Console.WriteLine($"one_test_to_get() is being ran at position: {testIndex}");

            // Act
            var result = await client.GetStringAsync(WebAppUrl);
            Console.WriteLine($"one_test_to_get() response: {result}");

            // Assert
            Assert.Equal($"I have been invoked {testIndex} times", result);
        }

        /// <summary>
        /// By the time this test runs, a fresh shared copy of the web application has been started up.
        ///
        /// This test may run first or second.
        /// </summary>
        [Fact]
        public async void another_test_to_get()
        {
            // Arrange
            var client = new HttpClient();
            var testIndex = ++_testFixture.TestExecutedCount;
            Console.WriteLine($"another_test_to_get is being ran at position: {testIndex}");

            // Act
            var result = await client.GetStringAsync(WebAppUrl);
            Console.WriteLine($"another_test_to_get() response: {result}");

            // Assert
            Assert.Equal($"I have been invoked {testIndex} times", result);
        }
    }
}
