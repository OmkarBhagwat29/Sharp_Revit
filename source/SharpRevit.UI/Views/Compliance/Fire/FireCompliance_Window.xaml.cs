using Microsoft.Extensions.Logging;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using SharpRevit.UI.ViewModels.FireCompliance;

using System.IO;
using System.Windows;



namespace SharpRevit.UI.Views.Compliance.Fire
{
    /// <summary>
    /// Interaction logic for FireCompliance.xaml
    /// </summary>
    public partial class FireCompliance_Window : Window
    {

        private bool _firstLoad = true;

        FireCompliance_ViewModel vm;
        public FireCompliance_Window(FireCompliance_ViewModel _vm)
        {

            InitializeComponent();
            this.InitializeAsync();
            vm = _vm;
            DataContext = _vm;
        }

        private async void InitializeAsync()
        {
            var path = Path.Combine(Path.GetTempPath(), "FireCompliance");
            var env = await CoreWebView2Environment.CreateAsync(
                userDataFolder: path,
                options: new CoreWebView2EnvironmentOptions(allowSingleSignOnUsingOSPrimaryAccount: true));
            await this.WebView.EnsureCoreWebView2Async(env);

            this.WebView.NavigationCompleted += WebView_NavigationCompleted;

            //WebView.Source = new Uri("https://www.google.com/");
        }


        private void WebView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (!_firstLoad)
                return;

            _firstLoad = false;


            this.vm.SetWebView(this.WebView);
        }



        private void Window_Closed(object sender, System.EventArgs e)
        {
            _firstLoad = true;
            this.WebView.Dispose();
        }

    }
}
