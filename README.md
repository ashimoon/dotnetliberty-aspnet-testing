# Description

Provides a way of starting and stopping an ASP.NET 5 web application for ease of use in service/integration tests.

Also contains a `WebApplicationTestFixture` that can be used to share a single web application instance amongst multiple test cases with xUnit.

## How

With ASP.NET 5, it is easy as pie to fire up a new web application programatically in a matter of seconds.

Ideally, we could use the built in `WebApplication.Run`, but this doesn't give us control over the application lifecycle. This library introduces a new class `WebApplicationTest` that behaves much in the same way, but gives us a `WebApplicationTestHarness` back that can be disposed to shut down the server cleanly.

## Why

This gives us the opportunity to write some cool integration tests, where the test runner is responsible for firing up its own copy of the web application to test against.

With a little bit of ingenuity, you could also override some services/dependencies that typically make integration testing hard or impractical. 

You can do this by inheriting from your application `Startup` class or providing a new one entirely when calling `WebApplicationTest.Run`.

This may not be a replacement for true, in-place service tests, but it does allow us to easily integration-test a substantial portion of our web applications. The possibilities are limitless. 

# Sample

## Minimal ASP.NET 5 App

```csharp
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
```

## xUnit Test

```csharp
[Fact]
public void Test()
{
    var url = "http://localhost:5151";
    var args = new [] {
        "--server",
        "Microsoft.AspNet.Server.Kestrel",
        "--server.urls",
        url
    };
    // Dispose shuts down server cleanly
    using (var harness = WebApplicationTest.Run<MinimalApplicationStartup>(args))
    {
        // Arrange
        var client = new HttpClient();

        // Act
        var result = await client.GetStringAsync(url);

        // Assert
        Assert.Equal("Hello World!", result);
    }
}
```

# More Samples

## xUnit - New Server Each Test 
(see /Samples/NewServerEachTest.cs)

This will fire up the test web application in the constructor of the xUnit test class. Then you can dispose of the server in the Dispose method.

xUnit will run the constructor and dispose method once for each test.

## xUnit - Single Server Multiple Tests 
(see /Samples/SingleServerMultipleTests.cs)

This will use xUnit class fixtures in order to fire up a single web application once and have it shared amongst all tests in the test class.

This is considerably faster than firing up a new web application instance for each test method.

# Notes

Since xUnit may run test classes in parallel, it is recommended to choose a different port for each test class.

# Website

.NET Liberty - http://dotnetliberty.com
