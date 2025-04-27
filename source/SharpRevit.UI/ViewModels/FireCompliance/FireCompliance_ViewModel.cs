
using Autodesk.Revit.UI;
using Microsoft.Extensions.Logging;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using Nice3point.Revit.Toolkit.External.Handlers;
using RevitCore.Compliance.FireSafety;
using RevitCore.Entities;
using RevitCore.Extensions;
using RevitCore.GeometryUtils;
using SharpRevit.UI.Services;
using SharpRevit.UI.Views.Compliance.Fire;

using System.Text.Json;


namespace SharpRevit.UI.ViewModels.FireCompliance
{
    public class FireCompliance_ViewModel : ObservableObject
    {
        private readonly ILogger _logger;
        public IWindowService _windowService;




        private readonly AsyncEventHandler _asyncExternalHandler = new();
        private readonly ActionEventHandler _externalHandler = new();



        public FireCompliance_ViewModel(ILogger<FireCompliance_ViewModel> logger,
            IWindowService windowService)
        {
            
            _windowService = windowService;
            _logger = logger;

            _externalHandler.Raise((app) =>
            {
                FCHelper.Sys = new FireSafetySystem(app.ActiveUIDocument.Document);
                
            });

        }

        private WebView2 _webView;
        public void SetWebView(WebView2 webView)
        {
            this._webView = webView;

            this._webView.WebMessageReceived += WebView_WebMessageReceived;

        }

        private void WebView_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            var json = e.WebMessageAsJson;
            using JsonDocument doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            string id = root.GetProperty("id").GetString();
            string command = root.GetProperty("command").GetString();

            try
            {
                switch (command)
                {
                    case "EVALUATE":
                        {

                            _asyncExternalHandler.RaiseAsync((uiApp) =>
                            {
                                FCHelper.Evaluate(uiApp.ActiveUIDocument.Document);
                                var data = FCHelper.GetReport();
                                var sendData = JsonSerializer.Serialize(new { eventType = "compliance_report", payload = data });
                                this._webView.CoreWebView2.PostWebMessageAsJson(sendData);

                            });
                        }
                        break;
                    case "RESET":
                        {
                            _externalHandler.Raise((uiApp) => { FCHelper.Reset(uiApp.ActiveUIDocument.Document); }); 
                        }
                        break;
                    case "TRAVEL_DISTANCE":
                        {
                            var distance = root.GetProperty("payload")
                            .GetProperty("value").GetDouble();

                           _asyncExternalHandler.RaiseAsync((uiApp)=> {

                               FCHelper.EvaluateTravelDistance(uiApp.ActiveUIDocument.Document, distance);

                               var data = FCHelper.GetReport();
                               var sendData = JsonSerializer.Serialize(new { eventType = "compliance_report", payload = data });
                               this._webView.CoreWebView2.PostWebMessageAsJson(sendData);

                           } );


                        }
                        break;
                }
                
            }
            catch
            {

            }
        }


    }
}
