using System;

namespace DotNetLiberty.AspNet.Testing
{
    /// <summary>
    /// xUnit test fixture for sharing a single web application instance amongst multiple test cases.
    ///
    /// xUnit class fixtures: http://xunit.github.io/docs/shared-context.html#class-fixture
    /// </summary>
    public abstract class WebApplicationTestFixture : IDisposable
    {
        private readonly WebApplicationTestHarness _harness;

        protected WebApplicationTestFixture(Type startupType, string[] args)
        {
            _harness = WebApplicationTest.Run(startupType, args);
        }

        public virtual void Dispose()
        {
            _harness.Dispose();
        }
    }

    /// <summary>
    /// xUnit test fixture for sharing a single web application instance amongst multiple test cases.
    ///
    /// xUnit class fixtures: http://xunit.github.io/docs/shared-context.html#class-fixture
    /// </summary>
    public abstract class WebApplicationTestFixture<TStartup> : WebApplicationTestFixture
    {
        protected WebApplicationTestFixture(string[] args)
            : base(typeof(TStartup), args)
        {
            
        } 

        protected WebApplicationTestFixture()
            : base(typeof(TStartup), null)
        {
            
        } 
    }
}