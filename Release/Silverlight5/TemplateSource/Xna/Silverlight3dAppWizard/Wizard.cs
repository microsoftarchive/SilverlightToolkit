using System.Collections.Generic;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TemplateWizard;
using EnvDTE;
using IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace Silverlight3dAppWizard
{
    class Wizard : IWizard
    {
        DTE2 globalDTE;
        public void BeforeOpeningFile(ProjectItem projectItem)
        {
        }

        public void ProjectFinishedGenerating(Project project)
        {
            Solution solution = globalDTE.Solution;

            // Getting specific projects
            Project silverlightProject = null;
            Project webProject = null;

            foreach (Project child in solution.Projects)
            {
                if (child.FileName.EndsWith("Silverlight3dWeb.csproj"))
                    webProject = child;
                else if (child.FileName.EndsWith("Silverlight3dApp.csproj"))
                    silverlightProject = child;
            }

            // Setting startup project
            solution.Properties.Item("StartupProject").Value = webProject.Name;

            webProject.Properties.Item("WebApplication.StartPageUrl").Value = "Silverlight3dAppTestPage.aspx";
            webProject.Properties.Item("WebApplication.DebugStartAction").Value = 1;

            // Rebuilding link between Silverlight project and Web project
            IServiceProvider sp = globalDTE as IServiceProvider;
            IVsSolution service;

            using (ServiceProvider provider2 = new ServiceProvider(sp))
            {
                service = provider2.GetService(typeof(IVsSolution)) as IVsSolution;
            }

            IVsHierarchy vsWebProjectHierarchy;
            IVsHierarchy vsSilverlightProjectHierarchy;

            if (
                (service.GetProjectOfUniqueName(webProject.UniqueName, out vsWebProjectHierarchy) == 0) 
                && 
                (vsWebProjectHierarchy != null)
                &&
                (service.GetProjectOfUniqueName(silverlightProject.UniqueName, out vsSilverlightProjectHierarchy) == 0)
                )
            {
                (vsWebProjectHierarchy as IVsSilverlightProjectConsumer).LinkToSilverlightProject("ClientBin", true, false, vsSilverlightProjectHierarchy as IVsSilverlightProject);
            }
        }

        public void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
        }

        public void RunFinished()
        {
        }

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            globalDTE = automationObject as DTE2;
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }
    }
}
