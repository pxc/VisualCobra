using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace VisualCobra.Visual_Cobra_Menu
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Guid("09ED5841-E5A2-475B-B869-42071B78320A")]
    class VisualCobraOptions : DialogPage
    {
        // use -ert (embed runtime) by default so you can run code in folders that don't
        // contain the Cobra DLL
        String _cobraCommandLine = "cobra -ert <filename>";
        public String CobraCommandLine
        {
            get { return _cobraCommandLine; }
            set { _cobraCommandLine = value; }
        }        
    }
}