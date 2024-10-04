using SharpRevit.config;
using SharpRevit.UI.Commands;
using SharpRevit.UI.Services;
using SharpRevit.UI.ViewModels;
using SharpRevit.UI.ViewModels.KeynotesCreationViewModel;
using SharpRevit.UI.ViewModels.SharedParametersViewModels;
using SharpRevit.UI.Views;

using SharpRevit.UI.Views.KeynotesCreationViews;
using SharpRevit.UI.Views.SharedParameterViews;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.IO;
using System.Reflection;


namespace SharpRevit
{
    /// <summary>
    ///     Provides a host for the application's services and manages their lifetimes
    /// </summary>
    public static class Host
    {
        private static IHost _host;

        public static void Start()
        {
            var builder = new HostApplicationBuilder(new HostApplicationBuilderSettings
            {
                ContentRootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                DisableDefaults = true
            });

            //logging
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilogConfiguration();

            builder.Services.AddSerializerOptions();

            //add here services like views and VM
             builder.Services.AddTransient<IWindowService, WindowService>();


            builder.Services.AddTransient<BrickEvaluatorShowWindow>();
            builder.Services.AddTransient<BrickWindow>();
            builder.Services.AddTransient<BrickEvaluator_ViewModel>();


            builder.Services.AddTransient<FamilyParameters_ViewModel>();
            builder.Services.AddTransient<FamilyParameters_Window>();
            builder.Services.AddTransient<FamilyParametersShowWindow>();


            builder.Services.AddTransient<KeynotesCreation_ViewModel>();
            builder.Services.AddTransient<KeynotesCreation_Window>();
            builder.Services.AddTransient<KeynotesCreationShowWindow>();


            _host = builder.Build();
            _host.Start();
        }

        /// <summary>
        ///     Stops the host
        /// </summary>
        public static void Stop()
        {
            _host.StopAsync();
        }

        /// <summary>
        ///     Gets a service of the specified type
        /// </summary>
        /// <typeparam name="T">The type of service object to get</typeparam>
        /// <returns>A service object of type T or null if there is no such service</returns>
        public static T GetService<T>() where T : class
        {
            return _host.Services.GetService(typeof(T)) as T;
        }
    }
}
