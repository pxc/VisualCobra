// Copyright (c) 2010 Matthew Strawbridge
// See accompanying licence.txt for licence details

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
        public const String multiLineCommentDelimiter = "\"\"\""; // three quotes

        /// <summary>
        /// Cache multi-line comment spans for each snapshot so we don't have to
        /// calculate them each time.
        /// NOTE: Because there is a separate classifier for each Cobra file in VS,
        /// _commentCache will always have at most one entry. But it's a Dictionary
        /// so we can more easily test that this assumption is true.
        /// </summary>
        protected IDictionary<ITextSnapshot, IList<Span>> _commentCache;

        private static IClassificationType _cobraKeywordClassificationType;
        private static IClassificationType _cobraCommentClassificationType;
        private static IClassificationType _cobraStringClassificationType;
        private static IClassificationType _cobraClassClassificationType;
        private static IClassificationType _cobraIndentErrorClassificationType;

        internal VisualCobra(IClassificationTypeRegistryService registry)
        {
            // initialise member data
            _commentCache = new Dictionary<ITextSnapshot, IList<Span>>();

            _cobraKeywordClassificationType = registry.GetClassificationType("CobraKeyword");
            _cobraCommentClassificationType = registry.GetClassificationType("CobraComment");
            _cobraStringClassificationType = registry.GetClassificationType("CobraString");
            _cobraClassClassificationType = registry.GetClassificationType("CobraClass");
            _cobraIndentErrorClassificationType = registry.GetClassificationType("CobraIndentError");
        }

        protected static bool isClass(IToken tok)
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
            if (_commentCache.ContainsKey(span.Snapshot))
                return _commentCache[span.Snapshot];

            List<Span> multiLineComments = new List<Span>();

            String text = span.Snapshot.GetText();

            int commentStart = 0 - multiLineCommentDelimiter.Length;
            int commentEnd = 0 - multiLineCommentDelimiter.Length;

            do
            {
                commentStart = text.IndexOf(multiLineCommentDelimiter, commentEnd + multiLineCommentDelimiter.Length);
                if (commentStart > -1)
                {
                    commentEnd = text.IndexOf(multiLineCommentDelimiter, commentStart + multiLineCommentDelimiter.Length);
                    if (commentEnd == -1)
                    {
                        commentEnd = text.Length; // if there's no end then run to end of file
                    }
                    else
                    {
                        commentEnd += multiLineCommentDelimiter.Length;
                    }
                    multiLineComments.Add(Span.FromBounds(commentStart, commentEnd));
                    Debug.Assert(commentEnd > commentStart);
                }
            } while (commentStart > -1 && commentEnd > -1 && commentEnd < text.Length);

            // Remove the cached version if there is one
            if (_commentCache.Count > 0)
            {
                ITextBuffer buffer = span.Snapshot.TextBuffer;
                Debug.Assert(_commentCache.Count == 1);
                // existing entry should refer to the same text buffer
                IEnumerator<ITextSnapshot> e = _commentCache.Keys.GetEnumerator();
                e.MoveNext();
                Debug.Assert(buffer.Equals(e.Current.TextBuffer));
                _commentCache.Remove(e.Current);
                Debug.Assert(_commentCache.Count == 0);
            }

            // add to cache so we don't have to calculate it again for this snapshot
            _commentCache[span.Snapshot] = multiLineComments;

            return multiLineComments;
        }

        /// <summary>
        /// Searches the suppled SnapShotSpan, classifying any indentation errors it contains
        /// (i.e. where the leading space is a mixture of tabs and spaces).
        /// </summary>
        /// <param name="span">The SnapshotSpan currently being classified</param>
        /// <returns>A list of error spans</returns>
        public IList<ClassificationSpan> GetIndentErrorSpans(SnapshotSpan span)
        {
            IList<ClassificationSpan> spans = new List<ClassificationSpan>();

            string text = span.GetText();

            Regex r = new Regex("(\t+ )|( +\t)");

            foreach (Match m in r.Matches(text))
            {
                spans.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, span.Start + m.Index + m.Length - 1, 1), _cobraIndentErrorClassificationType));
            }

            return spans;
        }


        /// <summary>
        /// This method scans the given SnapshotSpan for potential matches for this classification.
        /// </summary>
        /// <param name="trackingSpan">The span currently being classified</param>
        /// <returns>A list of ClassificationSpans that represent spans identified to be of this classification</returns>
        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            // TODO: this is an inefficient call to GetMultiLineComments each time!
            IList<Span> multiLineComments = GetMultiLineComments(span);

            //create a list to hold the results
            List<ClassificationSpan> classifications = new List<ClassificationSpan>();

            Compiler compiler = new Compiler();
            compiler.Options.Add("number", "float");

            // Use Cobra's own tokenizer
            VisualCobraTokenizer tokCobra = new VisualCobraTokenizer();
            tokCobra.TypeProvider = compiler;
            tokCobra.WillReturnComments = true;
            tokCobra.WillReturnDirectives = true;
            
            Node.SetCompiler(compiler);
            tokCobra.StartSource(span.GetText());
            
            List<IToken> toks = tokCobra.AllTokens();

            IToken previous = null;
            foreach (IToken tok in toks)
            {
                if (tok.IsKeyword)
                {
                    var tokenSpan = new SnapshotSpan(span.Snapshot, new Span(span.Start.Position + tok.CharNum - 1, tok.Length)); // +1
                    ClassificationSpan cs = new ClassificationSpan(tokenSpan, _cobraKeywordClassificationType);
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
                            {
                                // extend the span to include the surrounding single or double quotes
                                var tokenSpan = new SnapshotSpan(span.Snapshot, new Span(span.Start.Position + tok.CharNum - 1, tok.Length));
                                ClassificationSpan cs = new ClassificationSpan(tokenSpan, _cobraStringClassificationType);
                                classifications.Add(cs);
                            }
                            break;
                        case "ID": // Note "CLASS" is the class keyword, not "a class"
                            {

                                if (isClass(tok))
                                {
                                    // it starts upper case so, in Cobra, it's a class                                    
                                    var tokenSpan = new SnapshotSpan(span.Snapshot, new Span(span.Start.Position + tok.CharNum - 1, tok.Length));
                                    ClassificationSpan cs = new ClassificationSpan(tokenSpan, _cobraClassClassificationType);
                                    classifications.Add(cs);
                                }
                            }
                            break;
                        case "QUESTION":
                            {
                                if (isClass(previous))
                                {
                                    // add another char to cover the ? on the end of a nillable class                                   
                                    var tokenSpan = new SnapshotSpan(span.Snapshot, new Span(span.Start.Position + tok.CharNum - 1, 1));
                                    ClassificationSpan cs = new ClassificationSpan(tokenSpan, _cobraClassClassificationType);
                                    classifications.Add(cs);
                                }
                                break;
                            }
                        case "COMMENT":
                            {
                                var tokenSpan = new SnapshotSpan(span.Snapshot, new Span(span.Start.Position + tok.CharNum - 1, tok.Length));
                                ClassificationSpan cs = new ClassificationSpan(tokenSpan, _cobraCommentClassificationType);
                                classifications.Add(cs);
                            }
                            break;
                    }
                    previous = tok;
                }                
            }

            // Add classification spans for indent errors
            classifications.AddRange(GetIndentErrorSpans(span));

            List<ClassificationSpan> removeMe = new List<ClassificationSpan>();
            List<ClassificationSpan>.Enumerator e = classifications.GetEnumerator();
            while (e.MoveNext())
            {
                foreach (Span comment in multiLineComments)
                {
                    if (comment.Contains(e.Current.Span))
                    {
                        // Can't do this while looping through...
                        // classifications.Remove(e.Current);
                        // ... so add to a list and remove afterwards
                        removeMe.Add(e.Current);
                    }
                }
            }

            foreach (ClassificationSpan remove in removeMe)
            {
                classifications.Remove(remove);
            }

            // Add all comment spans
            // TODO: Many of these will be out of bounds, so they should perhaps be trimmed down first
            foreach (Span comment in multiLineComments)
            {
                classifications.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, comment), _cobraCommentClassificationType));
            }

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