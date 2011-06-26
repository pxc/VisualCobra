// Copyright (c) 2010-2011 Matthew Strawbridge
// See accompanying licence.txt for licence details

using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Media;

namespace VisualCobra.Classification
{
    #region Format definition
    /// <summary>
    /// Keywords are blue.
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "CobraKeyword")]
    [Name("CobraKeyword")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class CobraKeywordFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "CobraKeyword" classification type
        /// </summary>
        public CobraKeywordFormat()
        {
            DisplayName = "CobraKeywordFormat";
            ForegroundColor = Colors.Blue;
        }
    }

    /// <summary>
    /// Comments are green.
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "CobraComment")]
    [Name("CobraComment")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class CobraCommentFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "CobraComment" classification type
        /// </summary>
        public CobraCommentFormat()
        {
            DisplayName = "CobraCommentFormat";
            ForegroundColor = Colors.Green;
        }
    }

    /// <summary>
    /// Strings are brown.
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "CobraString")]
    [Name("CobraString")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class CobraStringFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "CobraString" classification type
        /// </summary>
        public CobraStringFormat()
        {
            DisplayName = "CobraStringFormat";
            ForegroundColor = Colors.Brown;
        }
    }

    /// <summary>
    /// Classes are light blue.
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "CobraClass")]
    [Name("CobraClass")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class CobraClassFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "CobraClass" classification type
        /// </summary>
        public CobraClassFormat()
        {
            DisplayName = "CobraClassFormat";
            ForegroundColor = Colors.CornflowerBlue;
        }
    }

    /// <summary>
    /// Indent errors (mixture of leading tabs and spaces) are underlined in orange.
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "CobraIndentError")]
    [Name("CobraIndentError")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class CobraIndentErrorFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "CobraIndentError" classification type
        /// </summary>
        public CobraIndentErrorFormat()
        {
            DisplayName = "CobraIndentErrorFormat";

            var cobraIndentErrorColor = Colors.Orange;
            const double cobraIndentErrorPenWidth = 1.0;

            var cobraIndentErrorUnderline = new TextDecoration();
            var orangePen = new Pen(new SolidColorBrush(cobraIndentErrorColor), cobraIndentErrorPenWidth) { DashStyle = DashStyles.Dot };

            cobraIndentErrorUnderline.Pen = orangePen;
            cobraIndentErrorUnderline.PenThicknessUnit = TextDecorationUnit.FontRecommended;

            var myCollection = new TextDecorationCollection { cobraIndentErrorUnderline };

            TextDecorations = myCollection;
        }
    }
    #endregion //Format definition
}