
using SharpRevit.Commands;
using Nice3point.Revit.Toolkit.External;

namespace SharpRevit.Tools
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

                this.CreateNeomBIMStartUp();

                //Create Tab
                string tabName = "Sharp Revit Tools";
                Application.CreateRibbonTab(tabName);

                string parameterPanelName = "Parameters";
  
                string GeneralPanelName = "General";
   

                var parametersPanel = Application.CreatePanel(parameterPanelName, tabName);

                var generalPanel = Application.CreatePanel(GeneralPanelName, tabName);


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

        void CreateNeomBIMStartUp()
        {
            try
            {
                string tabName = "NEO BIM";
                Application.CreateRibbonTab(tabName);

                string compliancePanelName = "Check Fire Safety";

                var panel = Application.CreatePanel(compliancePanelName, tabName);

                FireCompliance_Command.CreateFireComplianceButton(panel);
            }
            catch
            {

            }
        }

    }
}
