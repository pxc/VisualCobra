// Copyright (c) 2010-2012 Matthew Strawbridge
// See accompanying licence.txt for licence details

namespace VisualCobra
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Utilities;

    /// <summary>
    /// Sets up file and content type associations for Cobra.
    /// </summary>
    public class FileAndContentTypes
    {
        /// <summary>
        /// Associates the Cobra classifier with the .cobra file extension.
        /// </summary>
        internal static class FileAndContentTypeDefinitions
        {
            /// <summary>
            /// Specifies a content type definition for Cobra files.
            /// </summary>
            [Export]
            [Name("cobra")]
            [BaseDefinition("text")]
            internal static ContentTypeDefinition CobraContentTypeDefinition = null;

            /// <summary>
            /// Specifies that files with a .cobra extension are Cobra files.
            /// </summary>
            [Export]
            [FileExtension(".cobra")]
            [ContentType("cobra")]
            internal static FileExtensionToContentTypeDefinition CobraFileExtensionDefinition = null;
        }
    }
}
