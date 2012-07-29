// Copyright (c) 2010-2012 Matthew Strawbridge
// See accompanying licence.txt for licence details

// TODO Backticks in multi-line comments cause the line to appear black instead of green.
// TODO If the three """ appear in a string, the string takes precedence. E.g. r'[ \t]*"""[ \t]*\n' is a string not a comment.

namespace VisualCobra
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Classification;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;

    /// <summary>
    /// <para>Classifier for Cobra.</para>
    /// <para>Note that a new instance of this classifier is created for each Cobra file opened, so member variables only need to
    /// worry about a single document.</para>
    /// </summary>
    public sealed class VisualCobra : IClassifier
    {
        /// <summary>
        /// Three quotes, used to mark the beginning or end of a multi-line comment.
        /// </summary>
        public const string MultiLineCommentDelimiter = "\"\"\"";

        /// <summary>
        /// Classification type for Cobra keywords.
        /// </summary>
        private static IClassificationType _cobraKeywordClassificationType;

        /// <summary>
        /// Classification type for Cobra comments.
        /// </summary>
        private static IClassificationType _cobraCommentClassificationType;

        /// <summary>
        /// Classification type for Cobra strings.
        /// </summary>
        private static IClassificationType _cobraStringClassificationType;

        /// <summary>
        /// Classification type for Cobra classes.
        /// </summary>
        private static IClassificationType _cobraClassClassificationType;

        /// <summary>
        /// Classification type for Cobra indentation errors.
        /// </summary>
        private static IClassificationType _cobraIndentErrorClassificationType;

#pragma warning disable 1584,1711,1572,1581,1580
        /// <summary>
        /// Cache multi-line comment spans for each snapshot so we don't have to
        /// calculate them each time.
        /// </summary>
        /// <remarks>
        /// NOTE: Because there is a separate classifier for each Cobra file in VS,
        /// <see cref="_commentCache"/> will always have at most one entry. But it's an
        /// <see cref="System.Collections.Generic.IDictionary"/> so we can more easily test that this assumption is true.
        /// </remarks>
        private readonly IDictionary<ITextSnapshot, IList<Span>> _commentCache;
#pragma warning restore 1584,1711,1572,1581,1580

        /// <summary>
        /// Initializes a new instance of the VisualCobra class.
        /// </summary>
        /// <param name="registry">The registry.</param>
        internal VisualCobra(IClassificationTypeRegistryService registry)
        {
            _commentCache = new Dictionary<ITextSnapshot, IList<Span>>();

            _cobraKeywordClassificationType = registry.GetClassificationType("CobraKeyword");
            _cobraCommentClassificationType = registry.GetClassificationType("CobraComment");
            _cobraStringClassificationType = registry.GetClassificationType("CobraString");
            _cobraClassClassificationType = registry.GetClassificationType("CobraClass");
            _cobraIndentErrorClassificationType = registry.GetClassificationType("CobraIndentError");
        }

#pragma warning disable 67
        /// <summary>
        /// This event gets raised if a non-text change would affect the classification in some way,
        /// for example typing /* would cause the classification to change in C# without directly
        /// affecting the span.
        /// </summary>
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
#pragma warning restore 67

        /// <summary>
        /// This method scans the given SnapshotSpan for potential matches for this classification.
        /// </summary>
        /// <param name="span">The span currently being classified.</param>
        /// <returns>A list of ClassificationSpans that represent spans identified to be of this classification.</returns>
        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            // TODO this is an inefficient call to GetMultiLineComments each time!
            var multiLineComments = GetMultiLineComments(span.Snapshot);

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

            // Remove classification spans that are (partly) inside multi-line comment spans
            var multiLineCommentsInCurrentSpan = multiLineComments.Where(span.IntersectsWith).ToList();
            classifications.RemoveAll(cs => multiLineCommentsInCurrentSpan.AsParallel().Any(mlc => mlc.IntersectsWith(cs.Span.Span)));

            // Add all comment spans
            classifications.AddRange(
                multiLineCommentsInCurrentSpan.Select(
                    mlc =>
                    new ClassificationSpan(new SnapshotSpan(span.Snapshot, mlc), _cobraCommentClassificationType)));

            return classifications;
        }

        /// <summary>
        /// Determines whether the specified token represents the name of a class.
        /// </summary>
        /// <param name="tok">The token.</param>
        /// <returns>
        ///   <c>true</c> if the specified token is the name of a class; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsClass(IToken tok)
        {
            return tok != null && tok.Which == "ID" && tok.Text.Length > 0 && char.IsUpper(tok.Text[0]);
        }

        /// <summary>
        /// Searches the suppled SnapShotSpan, classifying any indentation errors it contains
        /// (i.e. where the leading space is a mixture of tabs and spaces).
        /// </summary>
        /// <param name="span">The span currently being classified.</param>
        /// <returns>A sequence of error spans.</returns>
        private static IEnumerable<ClassificationSpan> GetIndentErrorSpans(SnapshotSpan span)
        {
            var text = span.GetText();
            var r = new Regex("(\t+ )|( +\t)");
            return (from Match m in r.Matches(text)
                    select
                        new ClassificationSpan(
                        new SnapshotSpan(span.Snapshot, span.Start + m.Index + m.Length - 1, 1),
                        _cobraIndentErrorClassificationType)).ToList();
        }

        /// <summary>
        /// Creates a <see cref="ClassificationSpan"/> from the supplied span and token, and
        /// adds it to the supplied classifications.
        /// </summary>
        /// <param name="span">The snapshop span.</param>
        /// <param name="classifications">The classifications.</param>
        /// <param name="tok">The token.</param>
        /// <param name="classificationType">Type of the classification.</param>
        private static void AddSpanToClassifications(
            SnapshotSpan span,
            ICollection<ClassificationSpan> classifications,
            IToken tok,
            IClassificationType classificationType)
        {
            var tokenSpan = new SnapshotSpan(
                span.Snapshot, new Span(span.Start.Position + tok.CharNum - 1, tok.Length));
            var cs = new ClassificationSpan(tokenSpan, classificationType);
            classifications.Add(cs);
        }

        /// <summary>
        /// Gets the classifications from <see cref="VisualCobraTokenizer"/>.
        /// </summary>
        /// <param name="span">The span to get classifications from.</param>
        /// <param name="tokenizer">The tokenizer to use.</param>
        /// <returns>A list of <see cref="ClassificationSpan"/> objects representing the classifications
        /// in <paramref name="span"/>.</returns>
        private static List<ClassificationSpan> GetClassificationsFromCobraTokenizer(
            SnapshotSpan span, Tokenizer tokenizer)
        {
            // TODO Tried parallelising, but I'm not sure it's safe in combination with "previous".
            var classifications = new List<ClassificationSpan>();
            IToken previous = null;
            foreach (IToken tok in tokenizer.AllTokens())
            {
                if (tok.IsKeyword)
                {
                    AddSpanToClassifications(span, classifications, tok, _cobraKeywordClassificationType);
                }
                else
                {
                    switch (tok.Which)
                    {
                        case "STRING_SINGLE":
                        case "STRING_DOUBLE":
                        case "CHAR":
                        case "CHAR_LIT_SINGLE":
                            AddSpanToClassifications(
                                span, classifications, tok, _cobraStringClassificationType);
                            break;

                        case "ID": // Note "CLASS" is the class keyword, not "a class"
                            if (IsClass(tok))
                            {
                                AddSpanToClassifications(
                                    span, classifications, tok, _cobraClassClassificationType);
                            }

                            break;

                        case "QUESTION":
                            {
                                if (IsClass(previous))
                                {
                                    // add another char to cover the ? on the end of a nillable class
                                    AddSpanToClassifications(
                                        span, classifications, tok, _cobraClassClassificationType);
                                }

                                break;
                            }

                        case "COMMENT":
                            AddSpanToClassifications(
                                span, classifications, tok, _cobraCommentClassificationType);
                            break;
                    }

                    previous = tok;
                }
            }

            return classifications;
        }

        /// <summary>
        /// Quickly parses the entire text of the file and returns a span for each multi-line comment
        /// (because these cannot be determined from a snapshot without any context).
        /// </summary>
        /// <param name="snapshot">The text snapshot</param>
        /// <returns>A sequence of Spans (possibly empty) with one span for each multi-line comment in the file.</returns>
        private IEnumerable<Span> GetMultiLineComments(ITextSnapshot snapshot)
        {
            // use cache
            IList<Span> multiLineComments;
            _commentCache.TryGetValue(snapshot, out multiLineComments);
            if (multiLineComments != null)
            {
                return multiLineComments;
            }

            multiLineComments = new List<Span>();

            string text = snapshot.GetText();

            int commentStart;
            int commentEnd = 0 - MultiLineCommentDelimiter.Length;

            do
            {
                commentStart = text.IndexOf(
                    MultiLineCommentDelimiter, commentEnd + MultiLineCommentDelimiter.Length, StringComparison.Ordinal);

                if (commentStart < 0)
                {
                    continue;
                }

                commentEnd = text.IndexOf(
                    MultiLineCommentDelimiter, commentStart + MultiLineCommentDelimiter.Length, StringComparison.Ordinal);

                if (commentEnd < 0)
                {
                    commentEnd = text.Length; // if there's no end then run to end of file
                }
                else
                {
                    commentEnd += MultiLineCommentDelimiter.Length;
                }

                multiLineComments.Add(Span.FromBounds(commentStart, commentEnd));

                Debug.Assert(
                    commentStart < commentEnd,
                    "Comment must have some width and the start and end must be the correct way around");
            } while (commentStart > -1 && commentEnd > -1 && commentEnd < text.Length);

            // Remove the cached version if there is one
            if (_commentCache.Count > 0)
            {
                //// ReSharper disable RedundantAssignment
                ITextBuffer buffer = snapshot.TextBuffer;
                //// ReSharper restore RedundantAssignment

                Debug.Assert(_commentCache.Count == 1, "There should be one item in the cache");
                Debug.Assert(buffer.Equals(_commentCache.Single().Key.TextBuffer), "The key should be the same");

                _commentCache.Clear();
            }

            // add to cache so we don't have to calculate it again for this snapshot
            _commentCache[snapshot] = multiLineComments;

            return multiLineComments;
        }
    }
}
