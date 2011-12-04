// Copyright (c) 2010-2011 Matthew Strawbridge
// See accompanying licence.txt for licence details

namespace VisualCobra.Classification
{
    using System.ComponentModel.Composition;
    using System.Diagnostics;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Utilities;

    #region Provider definition
    /// <summary>
    /// This class causes a classifier representing Cobra files to be added to the set of classifiers.
    /// </summary>
    [Export(typeof(IClassifierProvider))]
    [ContentType("cobra")] // defined in FileAndContentTypes.cs
    internal class VisualCobraProvider : IClassifierProvider
    {
        /// <summary>
        /// Imports the classification registry to be used for getting a reference
        /// to the custom classification type later.
        /// </summary>
        [Import]
        internal IClassificationTypeRegistryService ClassificationRegistry; // Set via MEF

        /// <summary>
        /// Gets the classifier using the <see cref="ClassificationRegistry"/>.
        /// </summary>
        /// <param name="buffer">The buffer to assign the classifier to as a property.</param>
        /// <returns>A classifier.</returns>
        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            Trace.WriteLine("In GetClassifier()");
            return buffer.Properties.GetOrCreateSingletonProperty(
                () => new VisualCobra(this.ClassificationRegistry));
        }
    }
    #endregion //provider def
}