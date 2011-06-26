// Copyright (c) 2010-2011 Matthew Strawbridge
// See accompanying licence.txt for licence details

using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using VisualCobra.Classification;

// TODO: Backticks in multi-line comments cause the line to appear black instead of green.
// TODO: If the three """ appear in a string, the string takes precedence. E.g. r'[ \t]*"""[ \t]*\n' is a string not a comment.

namespace VisualCobra
{
    #region Classifier
    /// <summary>
    /// Classifier for Cobra.
    /// Note that a new instance of this classifier is created for each Cobra file opened, so member variables only need to
    /// worry about a single document.
    /// </summary>
    class VisualCobra : IClassifier
    {
        public const String MultiLineCommentDelimiter = "\"\"\""; // three quotes

        /// <summary>
        /// Cache multi-line comment spans for each snapshot so we don't have to
        /// calculate them each time.
        /// NOTE: Because there is a separate classifier for each Cobra file in VS,
        /// _commentCache will always have at most one entry. But it's a Dictionary
        /// so we can more easily test that this assumption is true.
        /// </summary>
        protected IDictionary<ITextSnapshot, IList<Span>> CommentCache;

        private static IClassificationType _cobraKeywordClassificationType;
        private static IClassificationType _cobraCommentClassificationType;
        private static IClassificationType _cobraStringClassificationType;
        private static IClassificationType _cobraClassClassificationType;
        private static IClassificationType _cobraIndentErrorClassificationType;

        internal VisualCobra(IClassificationTypeRegistryService registry)
        {
            // initialise member data
            CommentCache = new Dictionary<ITextSnapshot, IList<Span>>();

            _cobraKeywordClassificationType = registry.GetClassificationType("CobraKeyword");
            _cobraCommentClassificationType = registry.GetClassificationType("CobraComment");
            _cobraStringClassificationType = registry.GetClassificationType("CobraString");
            _cobraClassClassificationType = registry.GetClassificationType("CobraClass");
            _cobraIndentErrorClassificationType = registry.GetClassificationType("CobraIndentError");
        }

        protected static bool IsClass(IToken tok)
        {
            return tok != null && tok.Which == "ID" && tok.Text.Length > 0 && Char.IsUpper(tok.Text[0]);
        }

        /// <summary>
        /// Quickly parses the entire text of the file and returns a span for each multi-line comment
        /// (because these cannot be determined from a snapshot without any context).
        /// </summary>
        /// <param name="span">The span currently being classified</param>
        /// <returns>A list of Spans (possibly empty) with one span for each multi-line comment in the file.</returns>
        protected IList<Span> GetMultiLineComments(SnapshotSpan span)
        {
            // use cache
            if (CommentCache.ContainsKey(span.Snapshot))
                return CommentCache[span.Snapshot];

            var multiLineComments = new List<Span>();

            var text = span.Snapshot.GetText();

            // ReSharper disable RedundantAssignment
            var commentStart = 0 - MultiLineCommentDelimiter.Length;
            var commentEnd = 0 - MultiLineCommentDelimiter.Length;
            // ReSharper restore RedundantAssignment

            do
            {
                commentStart = text.IndexOf(MultiLineCommentDelimiter, commentEnd + MultiLineCommentDelimiter.Length);
                if (commentStart <= -1) continue;
                commentEnd = text.IndexOf(MultiLineCommentDelimiter, commentStart + MultiLineCommentDelimiter.Length);
                if (commentEnd == -1)
                {
                    commentEnd = text.Length; // if there's no end then run to end of file
                }
                else
                {
                    commentEnd += MultiLineCommentDelimiter.Length;
                }
                multiLineComments.Add(Span.FromBounds(commentStart, commentEnd));
                Debug.Assert(commentEnd > commentStart);
            } while (commentStart > -1 && commentEnd > -1 && commentEnd < text.Length);

            // Remove the cached version if there is one
            if (CommentCache.Count > 0)
            {
                // ReSharper disable RedundantAssignment
                var buffer = span.Snapshot.TextBuffer;
                // ReSharper restore RedundantAssignment
                Debug.Assert(CommentCache.Count == 1);
                // existing entry should refer to the same text buffer
                var e = CommentCache.Keys.GetEnumerator();
                e.MoveNext();
                Debug.Assert(buffer.Equals(e.Current.TextBuffer));
                CommentCache.Remove(e.Current);
                Debug.Assert(CommentCache.Count == 0);
            }

            // add to cache so we don't have to calculate it again for this snapshot
            CommentCache[span.Snapshot] = multiLineComments;

            return multiLineComments;
        }

        /// <summary>
        /// Searches the suppled SnapShotSpan, classifying any indentation errors it contains
        /// (i.e. where the leading space is a mixture of tabs and spaces).
        /// </summary>
        /// <param name="span">The SnapshotSpan currently being classified</param>
        /// <returns>A list of error spans</returns>
        public static IList<ClassificationSpan> GetIndentErrorSpans(SnapshotSpan span)
        {
            var text = span.GetText();
            var r = new Regex("(\t+ )|( +\t)");
            return (from Match m in r.Matches(text)
                    select new ClassificationSpan(new SnapshotSpan(span.Snapshot, span.Start + m.Index + m.Length - 1, 1), _cobraIndentErrorClassificationType)).ToList();
        }

        private static void AddSpanToClassifications(SnapshotSpan span, List<ClassificationSpan> classifications, IToken tok, int tokenLength, IClassificationType classificationType)
        {
            var tokenSpan = new SnapshotSpan(span.Snapshot, new Span(span.Start.Position + tok.CharNum - 1, tokenLength));
            var cs = new ClassificationSpan(tokenSpan, classificationType);
            classifications.Add(cs);
        }

        private static List<ClassificationSpan> GetClassificationsFromCobraTokenizer(SnapshotSpan span, VisualCobraTokenizer tokCobra)
        {
            // Tried parallelising, but I'm not sure it's safe in combination with "previous".
            var classifications = new List<ClassificationSpan>();
            IToken previous = null;
            foreach (var tok in tokCobra.AllTokens())
            {
                if (tok.IsKeyword)
                {
                    var tokenSpan = new SnapshotSpan(span.Snapshot, new Span(span.Start.Position + tok.CharNum - 1, tok.Length)); // +1
                    var cs = new ClassificationSpan(tokenSpan, _cobraKeywordClassificationType);
                    classifications.Add(cs);
                }
                else
                {
                    switch (tok.Which)
                    {
                        case "STRING_SINGLE":
                        case "STRING_DOUBLE":
                        case "CHAR":
                        case "CHAR_LIT_SINGLE":
                            AddSpanToClassifications(span, classifications, tok, tok.Length, _cobraStringClassificationType);
                            break;
                        case "ID": // Note "CLASS" is the class keyword, not "a class"
                            if (IsClass(tok))
                            {
                                AddSpanToClassifications(span, classifications, tok, tok.Length, _cobraClassClassificationType);
                            }
                            break;
                        case "QUESTION":
                            {
                                if (IsClass(previous))
                                {
                                    // add another char to cover the ? on the end of a nillable class
                                    AddSpanToClassifications(span, classifications, tok, 1, _cobraClassClassificationType);
                                }
                                break;
                            }
                        case "COMMENT":
                            AddSpanToClassifications(span, classifications, tok, tok.Length, _cobraCommentClassificationType);
                            break;
                    }
                    previous = tok;
                }
            }
            return classifications;
        }
        /// <summary>
        /// This method scans the given SnapshotSpan for potential matches for this classification.
        /// </summary>
        /// <param name="span">The span currently being classified</param>
        /// <returns>A list of ClassificationSpans that represent spans identified to be of this classification</returns>
        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            // TODO: this is an inefficient call to GetMultiLineComments each time!
            var multiLineComments = GetMultiLineComments(span);

            var compiler = new Compiler();
            compiler.Options.Add("number", "float");

            // Use Cobra's own tokenizer
            var tokCobra = new VisualCobraTokenizer
                               {
                                   TypeProvider = compiler,
                                   WillReturnComments = true,
                                   WillReturnDirectives = true
                               };

            Node.SetCompiler(compiler);
            tokCobra.StartSource(span.GetText());
            
            var classifications = GetClassificationsFromCobraTokenizer(span, tokCobra);

            // Add classification spans for indent errors
            classifications.AddRange(GetIndentErrorSpans(span));

            var removeMe = new List<ClassificationSpan>();
            var e = classifications.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current != null)
                {
                    removeMe.AddRange(from comment in multiLineComments
// ReSharper disable PossibleNullReferenceException
                                      where comment.Contains(e.Current.Span)
// ReSharper restore PossibleNullReferenceException
                                      select e.Current);
                }
            }

            foreach (var remove in removeMe)
            {
                classifications.Remove(remove);
            }

            // Add all comment spans
            // TODO: Many of these will be out of bounds, so they should perhaps be trimmed down first
            classifications.AddRange(multiLineComments.Select(comment => new ClassificationSpan(new SnapshotSpan(span.Snapshot, comment), _cobraCommentClassificationType)));

            return classifications;
        }

#pragma warning disable 67
        // This event gets raised if a non-text change would affect the classification in some way,
        // for example typing /* would cause the classification to change in C# without directly
        // affecting the span.
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
#pragma warning restore 67
    }
    #endregion //Classifier
}