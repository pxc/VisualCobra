// Copyright (c) 2010-2011 Matthew Strawbridge
// See accompanying licence.txt for licence details
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace VisualCobra
{
    /// <summary>
    /// Sets up file and content type associations for Cobra.
    /// </summary>
    class FileAndContentTypes
    {
        /// <summary>
        /// Associates the Cobra classifier with the .cobra file extension.
        /// </summary>
        internal static class FileAndContentTypeDefinitions
        {
            [Export]
            [Name("cobra")]
            [BaseDefinition("text")]
            internal static ContentTypeDefinition CobraContentTypeDefinition;

            [Export]
            [FileExtension(".cobra")]
            [ContentType("cobra")]
            internal static FileExtensionToContentTypeDefinition CobraFileExtensionDefinition;
        }
    }
}