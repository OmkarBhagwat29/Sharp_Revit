using Microsoft.Extensions.DependencyInjection;
using SharpRevit.UI.Utils;
using SharpRevit.UI.Views.Compliance.Fire;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpRevit.UI.Commands
{
    public class FireComplianceShowWindow(IServiceProvider serviceProvider)
    {

        public void Execute()
        {
            if (WindowController.Focus<FireCompliance_Window>()) return;

            var view = serviceProvider.GetService<FireCompliance_Window>();
            WindowController.Show(view, Process.GetCurrentProcess().MainWindowHandle);
        }

    }
}
