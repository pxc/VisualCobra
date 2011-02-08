using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
//using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

namespace VisualCobra.Visual_Cobra_Menu
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Guid("09ED5841-E5A2-475B-B869-42071B78320A")]
    class VisualCobraOptions : DialogPage
    {
        // use -ert (embed runtime) by default so you can run code in folders that don't
        // contain the Cobra DLL
        String cobraCommandLine = "cobra -ert <filename>";
        public String CobraCommandLine
        {
            get { return cobraCommandLine; }
            set { cobraCommandLine = value; }
        }        
    }
}