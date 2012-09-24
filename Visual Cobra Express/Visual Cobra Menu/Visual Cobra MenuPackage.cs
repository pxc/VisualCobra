// Copyright (c) 2010-2012 Matthew Strawbridge
// See accompanying licence.txt for licence details

namespace VisualCobra.Visual_Cobra_Menu
{
    using System;
    using System.ComponentModel.Design;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;

    /// <summary>
    /// <para>
    /// This is the class that implements the package exposed by this assembly.
    /// </para>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </para>
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.GuidVisualCobraMenuPkgString)]
    [ProvideOptionPage(typeof(VisualCobraOptions), "Visual Cobra Options", "Environment", 1000, 1001, true)]
    public sealed class VisualCobraMenuPackage : Package
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VisualCobraMenuPackage"/> class.
        /// </summary>
        public VisualCobraMenuPackage()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", ToString()));
        }

        /////////////////////////////////////////////////////////////////////////////
        // Overriden Package Implementation

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", ToString()));
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            var mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null == mcs)
            {
                return;
            }

            // Create the command for Cobra settings
            var menuCobraSettingsCommandID = new CommandID(
                GuidList.GuidVisualCobraMenuCmdSet, (int)PkgCmdIDList.CmdIDCobraSettings);
            var menuItemCobraSettings = new MenuCommand(DoCobraSettings, menuCobraSettingsCommandID);
            mcs.AddCommand(menuItemCobraSettings);

            // Create the command for CobraRun
            var menuCobraRunCommandID = new CommandID(
                GuidList.GuidVisualCobraMenuCmdSet, (int)PkgCmdIDList.CmdIDCobraRun);
            var menuItemCobraRun = new MenuCommand(DoCobraRun, menuCobraRunCommandID);
            mcs.AddCommand(menuItemCobraRun);
        }

        /// <summary>
        /// Displays the Cobra run dialog and does the run.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void DoCobraRun(object sender, EventArgs e)
        {
            // TODO Save all open files?

            // Development tools extensibility (DTE)
            var dte = (EnvDTE.DTE)GetService(typeof(EnvDTE.DTE));
            if (dte != null && dte.ActiveDocument != null)
            {
                // get the command to run from the options
                var options = GetDialogPage(typeof(VisualCobraOptions)) as VisualCobraOptions;
                if (options != null)
                {
                    var rawCommandLine = options.CobraCommandLine;
                    var fullCommandLine = rawCommandLine.Replace(
                        Resources.Filename_placeholder, String.Format("\"{0}\"", dte.ActiveDocument.Name));

                    using (var cobraProcess = new Process
                    {
                        StartInfo =
                        {
                            FileName = "cmd",
                            Arguments =
                                String.Format(
                                    "/k {0}", fullCommandLine),
                            CreateNoWindow = false,
                            WorkingDirectory = dte.ActiveDocument.Path
                        }
                    })
                    {
                        try
                        {
                            cobraProcess.Start();
                        }
                        catch (Exception ex)
                        {
                            SimpleCobraMessageBox(String.Format(Resources.Error_running_Cobra, ex.Message));
                        }
                    }
                }
            }
            else
            {
                // either dte is null or there is no active document
                SimpleCobraMessageBox(Resources.No_Cobra_source_file_loaded);
            }
        }

        /// <summary>
        /// Displays the Cobra Settings dialog page.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void DoCobraSettings(object sender, EventArgs e)
        {
            var dte = (EnvDTE.DTE)GetService(typeof(EnvDTE.DTE));
            if (dte != null)
            {
                dte.ExecuteCommand("Tools.Options", "09ED5841-E5A2-475B-B869-42071B78320A");
            }
            else
            {
                SimpleCobraMessageBox("Could not get DTE");
            }
        }

        /// <summary>
        /// This function is the callback used to execute a command when the a menu item is clicked.
        /// See the Initialize method to see how the menu item is associated to this function using
        /// the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        /// <param name="sender">The object calling back to the menu.</param>
        /// <param name="e">The event.</param>
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local
        private void MenuItemCallback(object sender, EventArgs e) // ReSharper restore UnusedParameter.Local
// ReSharper restore UnusedMember.Local
        {
            // Show a Message Box to prove we were here
            var uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
            var clsid = Guid.Empty;
            int result;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(
                uiShell.ShowMessageBox(
                    dwCompRole: 0,
                    rclsidComp: ref clsid,
                    pszTitle: "Visual Cobra Menu",
                    pszText:
                        string.Format(
                            CultureInfo.CurrentCulture,
                            "Inside {0}.MenuItemCallback() called from 0x{1:X}",
                            ToString(),
                            ((MenuCommand)sender).CommandID.ID),
                    pszHelpFile: string.Empty,
                    dwHelpContextID: 0,
                    msgbtn: OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    msgdefbtn: OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                    msgicon: OLEMSGICON.OLEMSGICON_INFO,
                    fSysAlert: 0,
                    pnResult: out result));
        }

        /// <summary>
        /// Displays a simple message box.
        /// </summary>
        /// <param name="message">The message to display.</param>
        private void SimpleCobraMessageBox(string message)
        {
            var uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
            var clsid = Guid.Empty;
            int result;
            uiShell.ShowMessageBox(
                dwCompRole: 0,
                rclsidComp: ref clsid,
                pszTitle: "Cobra",
                pszText: message,
                pszHelpFile: string.Empty,
                dwHelpContextID: 0,
                msgbtn: OLEMSGBUTTON.OLEMSGBUTTON_OK,
                msgdefbtn: OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                msgicon: OLEMSGICON.OLEMSGICON_INFO,
                fSysAlert: 0,
                pnResult: out result);
        }
    }
}
