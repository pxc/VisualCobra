// Copyright (c) 2010-2011 Matthew Strawbridge
// See accompanying licence.txt for licence details

namespace VisualCobra.Classification
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Utilities;

    /// <summary>
    /// Token classification types.
    /// </summary>
    internal static class VisualCobraType
    {
        /// <summary>
        /// Defines the "CobraKeyword" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("CobraKeyword")]
        internal static ClassificationTypeDefinition CobraKeywordType;

        /// <summary>
        /// Defines the "CobraComment" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("CobraComment")]
        internal static ClassificationTypeDefinition CobraCommentType;

        /// <summary>
        /// Defines the "CobraString" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("CobraString")]
        internal static ClassificationTypeDefinition CobraStringType;

        /// <summary>
        /// Defines the "CobraClass" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("CobraClass")]
        internal static ClassificationTypeDefinition CobraClassType;

        /// <summary>
        /// Defines the "CobraIndentError" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("CobraIndentError")]
        internal static ClassificationTypeDefinition CobraIndentErrorType;
    }
}