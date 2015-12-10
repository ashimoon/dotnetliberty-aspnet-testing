using System;

namespace DotNetLiberty.AspNet.Testing
{
    /// <summary>
    /// Entry point for running web applications.
    ///
    /// Returns a <see cref="WebApplicationTestHarness"/> that when disposed stops the web application.
    /// </summary>
    public static class WebApplicationTest
    {
        public static WebApplicationTestHarness Run(string[] args)
        {
            return Run(null, args);
        }

        public static WebApplicationTestHarness Run<TStartup>()
        {
            return Run(typeof (TStartup), null);
        }

        public static WebApplicationTestHarness Run<TStartup>(string[] args)
        {
            return Run(typeof (TStartup), args);
        }

        public static WebApplicationTestHarness Run(Type startupType)
        {
            return Run(startupType, null);
        }

        public static WebApplicationTestHarness Run(Type startupType, string[] args)
        {
            return new WebApplicationTestHarness(startupType, args);
        }
    }
}