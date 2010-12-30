// Copyright (c) 2010 Matthew Strawbridge
// See accompanying licence.txt for licence details

using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace VisualCobra
{
    class FileAndContentTypes
    {
        /// <summary>
        /// Associate the Cobra classifier with the .cobra file extension.
        /// </summary>
        internal static class FileAndContentTypeDefinitions
        {
            [Export]
            [Name("cobra")]
            [BaseDefinition("text")]
            internal static ContentTypeDefinition cobraContentTypeDefinition = null;

            [Export]
            [FileExtension(".cobra")]
            [ContentType("cobra")]
            internal static FileExtensionToContentTypeDefinition cobraFileExtensionDefinition = null;
        }
    }
}