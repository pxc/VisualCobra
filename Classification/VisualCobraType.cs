// Copyright (c) 2010 Matthew Strawbridge
// See accompanying licence.txt for licence details

using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace VisualCobra
{
    internal static class VisualCobraClassificationDefinition
    {
        /// <summary>
        /// Defines the "CobraKeyword" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("CobraKeyword")]
        internal static ClassificationTypeDefinition CobraKeywordType = null;

        /// <summary>
        /// Defines the "CobraComment" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("CobraComment")]
        internal static ClassificationTypeDefinition CobraCommentType = null;

        /// <summary>
        /// Defines the "CobraString" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("CobraString")]
        internal static ClassificationTypeDefinition CobraStringType = null;

        /// <summary>
        /// Defines the "CobraClass" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("CobraClass")]
        internal static ClassificationTypeDefinition CobraClassType = null;

        /// <summary>
        /// Defines the "CobraIndentError" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("CobraIndentError")]
        internal static ClassificationTypeDefinition CobraIndentErrorType = null;
    }
}