﻿



using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using SharpRevit.UI.Commands;
using SharpRevit.UI.ViewModels;
using Nice3point.Revit.Toolkit.External;
using RevitCore.Utils;
using RevitCore.Extensions;
using System.Reflection;

namespace SharpRevit.Commands
{
    /// <summary>
    ///     External command entry point invoked from the Revit interface
    /// </summary>
    [UsedImplicitly]
    [Transaction(TransactionMode.Manual)]
    public class FamilyParameters_Command : ExternalCommand
    {
        public override void Execute()
        {
			try
			{
                var app = Application;
                var uiApp = ExternalCommandData.Application;
                var uiDoc = uiApp.ActiveUIDocument;
                var doc = uiDoc.Document;

                Host.GetService<FamilyParametersShowWindow>().Execute();
            }
			catch
			{
                
			}
        }

        public static void CreateFamilyParametersButton(RibbonPanel panel)
        {
            var assembly = Assembly.GetExecutingAssembly();
            panel.AddItem(new PushButtonData(MethodBase.GetCurrentMethod().DeclaringType?.Name,
                $"Family\nShared Parameters", assembly.Location, MethodBase.GetCurrentMethod().DeclaringType?.FullName)
            {
                ToolTip = "Add and Delete Shared Parameters to families",
                LargeImage = ImageUtils.LoadImage(assembly, "SharedParameters_32x32.png")
            });

        }
    }
}
