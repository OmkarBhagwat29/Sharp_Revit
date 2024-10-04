
using SharpRevit.Commands;
using Nice3point.Revit.Toolkit.External;

namespace SharpRevit
{
    /// <summary>
    /// Application Entry Point
    /// </summary>
    [UsedImplicitly]
    public class Application : ExternalApplication
    {
        public override void OnStartup()
        {
            try
            {
                Host.Start();

                //Create Tab
                string tabName = "Sharp Revit Tools";
                Application.CreateRibbonTab(tabName);

                string parameterPanelName = "Parameters";
                string validationPanelName = "Apartment Validation";
                string GeneralPanelName = "General";
                string accPanelName = "ACC";

               var parametersPanel = Application.CreatePanel(parameterPanelName, tabName);

                var validationPanel = Application.CreatePanel(validationPanelName, tabName);

                var generalPanel = Application.CreatePanel(GeneralPanelName, tabName);


                var accPanel = Application.CreatePanel(accPanelName, tabName);


                FamilyParameters_Command.CreateFamilyParametersButton(parametersPanel);



                CreateKeynotes_Command.CreateKeynotesCreationButton(generalPanel);

                BrickEvaluator_Command.CreateBrickEvaluatorButton(generalPanel);

            }
            catch
            {

            }

        }

        public override void OnShutdown()
        {
            Host.Stop();
        }

    }
}
