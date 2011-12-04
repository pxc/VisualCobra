// Copyright (c) 2010-2011 Matthew Strawbridge
// See accompanying licence.txt for licence details

namespace VisualCobra.Classification
{
    using System.ComponentModel.Composition;
    using System.Windows;
    using System.Windows.Media;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Utilities;

    #region Format definition
    /// <summary>
    /// Keywords are blue.
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "CobraKeyword")]
    [Name("CobraKeyword")]
    [UserVisible(true)] // this should be visible to the end user
    [Order(Before = Priority.Default)] // set the priority to be after the default classifiers
    internal sealed class CobraKeywordFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CobraKeywordFormat"/> class, which
        /// defines the visual format for the <see cref="CobraKeyword"/> classification type.
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
    [UserVisible(true)] // this should be visible to the end user
    [Order(Before = Priority.Default)] // set the priority to be after the default classifiers
    internal sealed class CobraCommentFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CobraCommentFormat"/> class,
        /// which defines the visual format for the <see cref="CobraComment"/> classification type.
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
    [UserVisible(true)] // this should be visible to the end user
    [Order(Before = Priority.Default)] // set the priority to be after the default classifiers
    internal sealed class CobraStringFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CobraStringFormat"/> class,
        /// which defines the visual format for the <see cref="CobraString"/> classification type.
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
    [UserVisible(true)] // this should be visible to the end user
    [Order(Before = Priority.Default)] // set the priority to be after the default classifiers
    internal sealed class CobraClassFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CobraClassFormat"/> class,
        /// which defines the visual format for the <see cref="CobraClass"/> classification type.
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
    [UserVisible(true)] // this should be visible to the end user
    [Order(Before = Priority.Default)] // set the priority to be after the default classifiers
    internal sealed class CobraIndentErrorFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CobraIndentErrorFormat"/> class,
        /// which defines the visual format for the <see cref="CobraIndentError"/> classification type.
        /// </summary>
        public CobraIndentErrorFormat()
        {
            DisplayName = "CobraIndentErrorFormat";

            var cobraIndentErrorColor = Colors.Orange;
            const double CobraIndentErrorPenWidth = 1.0;

            var cobraIndentErrorUnderline = new TextDecoration();
            var orangePen = new Pen(new SolidColorBrush(cobraIndentErrorColor), CobraIndentErrorPenWidth) { DashStyle = DashStyles.Dot };

            cobraIndentErrorUnderline.Pen = orangePen;
            cobraIndentErrorUnderline.PenThicknessUnit = TextDecorationUnit.FontRecommended;

            var myCollection = new TextDecorationCollection { cobraIndentErrorUnderline };

            TextDecorations = myCollection;
        }
    }
    #endregion //Format definition
}