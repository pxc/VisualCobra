// Guids.cs
// MUST match guids.h
using System;

namespace VisualCobra.Visual_Cobra_Menu
{
    static class GuidList
    {
        public const string GuidVisualCobraMenuPkgString = "da390a51-3a52-4137-8450-582915e57e8c";
        public const string GuidVisualCobraMenuCmdSetString = "c873ef57-927c-4728-a23a-8385d02dbeca";

        public static readonly Guid GuidVisualCobraMenuCmdSet = new Guid(GuidVisualCobraMenuCmdSetString);
    };
}