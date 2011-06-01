// Copyright (c) 2010-2011 Matthew Strawbridge
// See accompanying licence.txt for licence details

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace VisualCobra.Classification
{
    #region Provider definition
    /// <summary>
    /// This class causes a classifier to be added to the set of classifiers. Since 
    /// the content type is set to "text", this classifier applies to all text files
    /// </summary>
    [Export(typeof(IClassifierProvider))]
    [ContentType("cobra")] // defined in FileAndContentTypes.cs. TODO: change comment above
    internal class VisualCobraProvider : IClassifierProvider
    {
        /// <summary>
        /// Import the classification registry to be used for getting a reference
        /// to the custom classification type later.
        /// </summary>
        [Import]
        internal IClassificationTypeRegistryService ClassificationRegistry; // Set via MEF

        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            //Trace.WriteLine("In GetClassifier()");
            return buffer.Properties.GetOrCreateSingletonProperty(
                () => new VisualCobra(ClassificationRegistry));
        }
    }
    #endregion //provider def
}