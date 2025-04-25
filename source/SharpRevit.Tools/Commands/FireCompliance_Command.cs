



using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Nice3point.Revit.Toolkit.External;
using RevitCore.Utils;
using SharpRevit.UI.Commands;
using System.Reflection;


namespace SharpRevit.Commands
{
    /// <summary>
    ///     External command entry point invoked from the Revit interface
    /// </summary>
    [UsedImplicitly]
    [Transaction(TransactionMode.Manual)]
    public class FireCompliance_Command : ExternalCommand
    {

        public override void Execute()
        {
			try
			{
                var app = Application;
                var uiApp = ExternalCommandData.Application;
                var uiDoc = uiApp.ActiveUIDocument;
                var doc = uiDoc.Document;

                //TaskDialog.Show("Fire Compliant", "Hello Fire, Are you compliant");

                Host.GetService<FireComplianceShowWindow>().Execute();

            }
			catch
			{
                
			}
        }


        public static void CreateFireComplianceButton(RibbonPanel panel)
        {
            var assembly = Assembly.GetExecutingAssembly();
            panel.AddItem(new PushButtonData(MethodBase.GetCurrentMethod().DeclaringType?.Name,
                $"Compliance", assembly.Location, MethodBase.GetCurrentMethod().DeclaringType?.FullName)
            {
                ToolTip = "check fire safety compliance",
                LargeImage = ImageUtils.LoadImage(assembly, "FireCompliance_28x28.png")
            });

        }
    }
}
