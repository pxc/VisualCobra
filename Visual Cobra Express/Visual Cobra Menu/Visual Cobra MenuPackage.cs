using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;

namespace VisualCobra.Visual_Cobra_Menu
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidVisual_Cobra_MenuPkgString)]
    [ProvideOptionPage(typeof(VisualCobraOptions),
    "Visual Cobra Options", "Environment",
    1000, 1001, true)]
    public sealed class Visual_Cobra_MenuPackage : Package
    {
        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public Visual_Cobra_MenuPackage()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }



        /////////////////////////////////////////////////////////////////////////////
        // Overriden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Trace.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                // Create the command for Cobra settings
                CommandID menuCobraSettingsCommandID = new CommandID(GuidList.guidVisual_Cobra_MenuCmdSet, (int)PkgCmdIDList.cmdidCobraSettings);
                MenuCommand menuItemCobraSettings = new MenuCommand(DoCobraSettings, menuCobraSettingsCommandID);
                mcs.AddCommand(menuItemCobraSettings);

                // Create the command for CobraRun
                CommandID menuCobraRunCommandID = new CommandID(GuidList.guidVisual_Cobra_MenuCmdSet, (int)PkgCmdIDList.cmdidCobraRun);
                MenuCommand menuItemCobraRun = new MenuCommand(DoCobraRun, menuCobraRunCommandID);
                mcs.AddCommand(menuItemCobraRun);
            }
        }
        #endregion

        /// <summary>
        /// This function is the callback used to execute a command when the a menu item is clicked.
        /// See the Initialize method to see how the menu item is associated to this function using
        /// the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            // Show a Message Box to prove we were here
            IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
            Guid clsid = Guid.Empty;
            int result;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(
                       0,
                       ref clsid,
                       "Visual Cobra Menu",
                       string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback() called from 0x{1:X}", this.ToString(), ((MenuCommand)sender).CommandID.ID),
                       string.Empty,
                       0,
                       OLEMSGBUTTON.OLEMSGBUTTON_OK,
                       OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                       OLEMSGICON.OLEMSGICON_INFO,
                       0,        // false
                       out result));
        }

        private void DoCobraSettings(object sender, EventArgs e)
        {
            EnvDTE.DTE dte = (EnvDTE.DTE)GetService(typeof(EnvDTE.DTE));
            if (dte != null)
            {
                //dte.ExecuteCommand("Tools.Options");
                dte.ExecuteCommand("Tools.Options", "09ED5841-E5A2-475B-B869-42071B78320A");
            }
            else
            {
                // dte is null
                SimpleCobraMessageBox("Could not get DTE");
            }
        }

        private void DoCobraRun(object sender, EventArgs e)
        {
            // TODO: Save all open files?

            // Development tools extensibility (DTE)
            EnvDTE.DTE dte = (EnvDTE.DTE)GetService(typeof(EnvDTE.DTE));
            if (dte != null && dte.ActiveDocument != null)
            {
                // get the command to run from the options
                VisualCobraOptions options = GetDialogPage(typeof(VisualCobraOptions)) as VisualCobraOptions;
                String rawCommandLine = options.CobraCommandLine;
                String fullCommandLine = rawCommandLine.Replace("<filename>", '"' + dte.ActiveDocument.Name + '"');

                Process cobraProcess = new Process();
                cobraProcess.StartInfo.FileName = "cmd";
                cobraProcess.StartInfo.Arguments = "/k " + fullCommandLine; // /k prevents window closing on termination
                cobraProcess.StartInfo.CreateNoWindow = false;

                // switch to the source folder to run
                cobraProcess.StartInfo.WorkingDirectory = dte.ActiveDocument.Path;

                try
                {
                    cobraProcess.Start();
                }
                catch (Exception ex)
                {
                    SimpleCobraMessageBox("Error running Cobra: " + ex.Message);
                }
            }
            else
            {
                // either dte is null or there is no active document
                SimpleCobraMessageBox("No Cobra source file loaded");
            }
        }

        private void SimpleCobraMessageBox(String msg)
        {
            IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
            Guid clsid = Guid.Empty;
            int result;
            uiShell.ShowMessageBox(0,
                                   ref clsid,
                                   "Cobra",
                                   msg,
                                   string.Empty,
                                   0,
                                   OLEMSGBUTTON.OLEMSGBUTTON_OK,
                                   OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                                   OLEMSGICON.OLEMSGICON_INFO,
                                   0,        // false
                                   out result);
        }
    
    }
}