// Copyright (c) 2010-2012 Matthew Strawbridge
// See accompanying licence.txt for licence details

namespace VisualCobra.Visual_Cobra_Menu
{
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;

    /// <summary>
    /// The Options dialog page for Cobra.
    /// </summary>
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Guid("09ED5841-E5A2-475B-B869-42071B78320A")]
    public class VisualCobraOptions : DialogPage
    {
        /// <summary>
        /// The command line to use for running the Cobra executable.
        /// </summary>
        /// <remarks>
        /// Uses <c>-ert</c> (embed runtime) by default so you can run code in folders that don't
        /// contain the Cobra DLL.
        /// </remarks>
        private string _cobraCommandLine = Resources.Default_Cobra_command_line;

        /// <summary>
        /// Gets or sets the Cobra command line.
        /// </summary>
        /// <value>
        /// The Cobra command line.
        /// </value>
        public string CobraCommandLine
        {
            get { return _cobraCommandLine; }
            set { _cobraCommandLine = value; }
        }
    }
}
