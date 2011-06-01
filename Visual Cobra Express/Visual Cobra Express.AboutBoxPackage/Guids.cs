// Guids.cs
// MUST match guids.h
using System;

namespace Visual_Cobra_Express.AboutBoxPackage
{
    static class GuidList
    {
        public const string GuidAboutBoxPackagePkgString = "fd518d22-4ab4-4240-a247-b1bc835509be";
        public const string GuidAboutBoxPackageCmdSetString = "8cd97468-fd40-43d8-9d75-296049b8d162";

        public static readonly Guid GuidAboutBoxPackageCmdSet = new Guid(GuidAboutBoxPackageCmdSetString);
    };
}