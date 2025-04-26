
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
                            FCHelper.Evaluate();

                            _externalHandler.Raise((uiApp) =>
                            {
                                Door.BakeDoorsDirectionLine(uiApp.ActiveUIDocument.Document,
                                    [.. FCHelper.Sys.RoomsDoorsRelations
                                    .SelectMany(r => r.GetFilteredDoorsByDirection())]);


                                uiApp.ActiveUIDocument.Document.UseTransaction(() =>
                                {
                                    //Door.Solids.ForEach(s => s.Visualize(uiApp.ActiveUIDocument.Document));

                                    Door.Curves.ForEach(c =>
                                    {

                                        var cv = c.CreateDetailCurve(uiApp.ActiveUIDocument.Document, uiApp.ActiveUIDocument.ActiveView);
                                    });

                                    Door.Points.ForEach(p =>
                                    {

                                        p.VisualizePosition(uiApp.ActiveUIDocument.Document);
                                    });

                                }, "Bake");

                            });

                            var data = FCHelper.GetReport();
                            var sendData = JsonSerializer.Serialize(new { eventType = "compliance_report", payload = data });
                            this._webView.CoreWebView2.PostWebMessageAsJson(sendData);
                        } 
                        break;
                    case "RESET":
                        {
                            FCHelper.Reset();
                        }
                        break;
                    case "TRAVEL_DISTANCE":
                        {
                            var distance = root.GetProperty("payload")
                            .GetProperty("value").GetDouble();

                           _asyncExternalHandler.RaiseAsync((uiApp)=> {

                               //TaskDialog.Show("Distance", $"New Distance is: {distance}");

                           } );

                            FCHelper.EvaluateTravelDistance(distance);

                            var data = FCHelper.GetReport();
                            var sendData = JsonSerializer.Serialize(new { eventType = "compliance_report", payload = data });
                            this._webView.CoreWebView2.PostWebMessageAsJson(sendData);
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
