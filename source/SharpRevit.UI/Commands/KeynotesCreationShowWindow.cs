using SharpRevit.UI.Utils;
using SharpRevit.UI.Views.KeynotesCreationViews;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpRevit.UI.Commands
{
    public class KeynotesCreationShowWindow(IServiceProvider serviceProvider)
    {
        public void Execute()
        {

            if (WindowController.Focus<KeynotesCreation_Window>()) return;

            var view = serviceProvider.GetService<KeynotesCreation_Window>();
            WindowController.Show(view, Process.GetCurrentProcess().MainWindowHandle);
        }
    }
}
