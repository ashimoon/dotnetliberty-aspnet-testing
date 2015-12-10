using System;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Hosting.Internal;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.Server.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetLiberty.AspNet.Testing
{
    /// <summary>
    /// Mostly the same as <see cref="Microsoft.AspNet.Hosting.WebApplication"/> except the server is
    /// shut down when dispose is called, rather than waiting for CTRL+C.
    ///
    /// Adapted from https://github.com/aspnet/Hosting/blob/dev/src/Microsoft.AspNet.Hosting/WebApplication.cs
    /// </summary>
    public class WebApplicationTestHarness : IDisposable
    {
        protected const string HostingJsonFile = "hosting.json";
        protected const string ConfigFileKey = "config";
        protected const string EnvironmentVariablesPrefix = "ASPNET_";

        private readonly IApplication _app;

        /// <summary>
        /// Adapted from https://github.com/aspnet/Hosting/blob/dev/src/Microsoft.AspNet.Hosting/WebApplication.cs
        /// </summary>
        /// <param name="startupType"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public WebApplicationTestHarness(Type startupType, string[] args)
        {
            var tempBuilder = new ConfigurationBuilder().AddCommandLine(args);
            var tempConfig = tempBuilder.Build();
            var configFilePath = tempConfig[ConfigFileKey] ?? HostingJsonFile;
            var config = LoadHostingConfiguration(configFilePath, args);

            var hostBuilder = new WebHostBuilder(config, captureStartupErrors: true);
            if (null != startupType)
            {
                hostBuilder.UseStartup(startupType);
            }
            var host = hostBuilder.Build();

            Console.WriteLine("Starting test web application.");
            _app = host.Start();

            var hostingEnv = _app.Services.GetRequiredService<IHostingEnvironment>();
            Console.WriteLine($"Hosting environment: {hostingEnv.EnvironmentName}");

            var serverAddresses = _app.ServerFeatures.Get<IServerAddressesFeature>();
            if (null != serverAddresses)
            {
                foreach (var address in serverAddresses.Addresses)
                {
                    Console.WriteLine($"Now listening on: {address}");
                }
            }
            Console.WriteLine("Test web application started.");
        }

        public void Dispose()
        {
            Console.WriteLine("Stopping test web application.");
            var appLifetime = _app.Services.GetRequiredService<IApplicationLifetime>();
            appLifetime.StopApplication();
            _app.Dispose();
            Console.WriteLine("Waiting for test web application to shutdown.");
            appLifetime.ApplicationStopping.WaitHandle.WaitOne();
            appLifetime.ApplicationStopped.WaitHandle.WaitOne();
        }

        /// <summary>
        /// Copied from https://github.com/aspnet/Hosting/blob/dev/src/Microsoft.AspNet.Hosting/WebApplication.cs
        /// </summary>
        /// <param name="configJsonPath"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        internal static IConfiguration LoadHostingConfiguration(string configJsonPath, string[] args)
        {
            // We are adding all environment variables first and then adding the ASPNET_ ones
            // with the prefix removed to unify with the command line and config file formats
            return new ConfigurationBuilder()
                .AddJsonFile(configJsonPath, optional: true)
                .AddEnvironmentVariables()
                .AddEnvironmentVariables(prefix: EnvironmentVariablesPrefix)
                .AddCommandLine(args)
                .Build();
        }

    }
}
