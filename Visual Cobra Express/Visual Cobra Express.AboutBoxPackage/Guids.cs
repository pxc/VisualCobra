// Guids.cs
// MUST match guids.h
//
// Copyright (c) 2010-2012 Matthew Strawbridge
// See accompanying licence.txt for licence details

namespace Visual_Cobra_Express.AboutBoxPackage
{
    using System;

    /// <summary>
    /// GUIDs for the Visual Cobra Express AboutBox package.
    /// </summary>
    internal static class GuidList
    {
        /// <summary>
        /// The GUID of the about box package.
        /// </summary>
        public const string GuidAboutBoxPackagePkgString = "fd518d22-4ab4-4240-a247-b1bc835509be";

        /// <summary>
        /// The GUID of the about box package command set.
        /// </summary>
        public const string GuidAboutBoxPackageCmdSetString = "8cd97468-fd40-43d8-9d75-296049b8d162";

        /// <summary>
        /// The GUID of the about box package command set.
        /// </summary>
        public static readonly Guid GuidAboutBoxPackageCmdSet = new Guid(GuidAboutBoxPackageCmdSetString);
    }
}
