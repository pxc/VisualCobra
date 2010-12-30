// Guids.cs
// MUST match guids.h
using System;

namespace Visual_Cobra_Express.AboutBoxPackage
{
    static class GuidList
    {
        public const string guidAboutBoxPackagePkgString = "fd518d22-4ab4-4240-a247-b1bc835509be";
        public const string guidAboutBoxPackageCmdSetString = "8cd97468-fd40-43d8-9d75-296049b8d162";

        public static readonly Guid guidAboutBoxPackageCmdSet = new Guid(guidAboutBoxPackageCmdSetString);
    };
}