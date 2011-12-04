// Copyright (c) 2010-2011 Matthew Strawbridge
// See accompanying licence.txt for licence details

namespace VisualCobra.Classification
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// A tokenizer built on top of <see cref="CobraTokenizer"/>, but less interested
    /// in errors.
    /// </summary>
    public class VisualCobraTokenizer : CobraTokenizer
    {
        /// <summary>
        /// Called when an indentation consisting of a mixture of tabs and spaces is encountered.
        /// </summary>
        /// <param name="tok">The token.</param>
        /// <returns>
        /// The token returned by <see cref="CobraTokenizer.OnINDENT_MIXED_TS"/> or <c>null</c>
        /// if the base class throws a <see cref="TokenizerError"/>.
        /// </returns>
        public override IToken OnINDENT_MIXED_TS(IToken tok)
        {
            // Do everything the base class does except throwing an exception
            try
            {
                return base.OnINDENT_MIXED_TS(tok);
            }
            catch (TokenizerError e)
            {
                Trace.WriteLine(String.Format("Tokenizer error in OnINDENT_MIXED_TS: {0}", e.Message));
            }

            return null;
        }

        /// <summary>
        /// Called when an indentation consisting of all spaces is encountered.
        /// </summary>
        /// <param name="tok">The token.</param>
        /// <returns>
        /// The token returned by <see cref="CobraTokenizer.OnINDENT_ALL_SPACES"/> or <c>null</c>
        /// if the base class throws a <see cref="TokenizerError"/>.
        /// </returns>
        public override IToken OnINDENT_ALL_SPACES(IToken tok)
        {
            // Do everything the base class does except throwing an exception
            try
            {
                return base.OnINDENT_ALL_SPACES(tok);
            }
            catch (TokenizerError e)
            {
                Trace.WriteLine(String.Format("Tokenizer error in OnINDENT_ALL_SPACES: {0}", e.Message));
            }

            return null;
        }

        /// <summary>
        /// Retrieves all tokens.
        /// </summary>
        /// <returns>
        /// All tokens returned by <see cref="CobraTokenizer.AllTokens"/>, or <c>null</c>
        /// if the base class throws an exception.
        /// </returns>
        public override List<IToken> AllTokens()
        {
            var tokens = new List<IToken>();
            try
            {
                tokens.AddRange(base.AllTokens());
            }
            catch (Exception exc)
            {
                // just log it and carry on
                Trace.WriteLine(exc);
            }

            return tokens;
        }
    }
}