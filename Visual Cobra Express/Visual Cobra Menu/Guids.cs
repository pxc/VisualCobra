// Guids.cs
// MUST match guids.h

namespace VisualCobra.Visual_Cobra_Menu
{
    using System;

    /// <summary>
    /// Various GUIDs used in the Cobra menu.
    /// </summary>
    internal static class GuidList
    {
        /// <summary>
        /// GUID string for the Visual Cobra Menu package.
        /// </summary>
        public const string GuidVisualCobraMenuPkgString = "da390a51-3a52-4137-8450-582915e57e8c";

        /// <summary>
        /// GUID string for the Visual Cobra Menu command set.
        /// </summary>
        public const string GuidVisualCobraMenuCmdSetString = "c873ef57-927c-4728-a23a-8385d02dbeca";

        /// <summary>
        /// GUID for the Visual Cobra Menu command set.
        /// </summary>
        public static readonly Guid GuidVisualCobraMenuCmdSet = new Guid(GuidVisualCobraMenuCmdSetString);
    }
}