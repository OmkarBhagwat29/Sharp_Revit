﻿using SharpRevit.UI.Views;
using Microsoft.Extensions.DependencyInjection;
using SharpRevit.UI.Utils;
using System.Diagnostics;


namespace SharpRevit.UI.Commands
{
    /// <summary>
    ///     Command entry point invoked from the Revit AddIn Application
    /// </summary>
    public class BrickEvaluatorShowWindow(IServiceProvider serviceProvider)
    {
        public void Execute()
        {
            
            if (WindowController.Focus<BrickWindow>()) return;

            var view = serviceProvider.GetService<BrickWindow>();
            WindowController.Show(view, Process.GetCurrentProcess().MainWindowHandle);
        }
    }
}
