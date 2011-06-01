// Copyright (c) 2010-2011 Matthew Strawbridge
// See accompanying licence.txt for licence details

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace VisualCobra.Classification
{
    class VisualCobraTokenizer:CobraTokenizer
    {
        public override IToken OnINDENT_MIXED_TS(IToken tok)
        {
            // Do everything the base class does except throwing an exception
            IToken result = null;
            try
            {
                result = base.OnINDENT_MIXED_TS(tok);
            }
            catch (TokenizerError)
            {
                // Do nothing
            }
            return result;
        }

        public override IToken OnINDENT_ALL_SPACES(IToken tok)
        {
            // Do everything the base class does except throwing an exception
            IToken result = null;
            try
            {
                result = base.OnINDENT_ALL_SPACES(tok);
            }
            catch (TokenizerError)
            {
                // Do nothing
            }
            return result;
        }

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