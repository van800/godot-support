using JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Parsing.TokenNodeTypes;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.Text;

namespace JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Parsing
{
    public partial class TscnLexerGenerated
    {
        private static readonly LexerDictionary<TokenNodeType> ourKeywords = new LexerDictionary<TokenNodeType>();
        private readonly ReusableBufferRange myBuffer = new ReusableBufferRange();

        static TscnLexerGenerated()
        {
            foreach (var nodeType in TscnTokenNodeTypes.KEYWORDS)
            {
                var keyword = (TokenNodeType) nodeType;
                if (!TscnTokenNodeTypes.CONTEXTUAL_KEYWORDS[keyword])
                {
                    ourKeywords[keyword.TokenRepresentation] = keyword;
                }
            }
        }

        private TokenNodeType FindKeywordByCurrentToken()
        {
            return ourKeywords.GetValueSafe(myBuffer, yy_buffer, yy_buffer_start, yy_buffer_end);
        }

        private TokenNodeType myCurrentTokenType = null;

        private struct TokenPosition
        {
            public TokenNodeType CurrentTokenType;
            public int YyBufferIndex;
            public int YyBufferStart;
            public int YyBufferEnd;
            public int YyLexicalState;
        }

        public void Start()
        {
            Start(0, yy_buffer.Length, YYINITIAL);
        }

        public void Start(int startOffset, int endOffset, uint state)
        {
            yy_buffer_index = startOffset;
            yy_buffer_start = startOffset;
            yy_buffer_end = startOffset;
            yy_eof_pos = endOffset;
            yy_lexical_state = (int) state;
            myCurrentTokenType = null;
        }

        public void Advance()
        {
            myCurrentTokenType = null;
            LocateToken();
        }

        public object CurrentPosition
        {
            get
            {
                TokenPosition tokenPosition;
                tokenPosition.CurrentTokenType = myCurrentTokenType;
                tokenPosition.YyBufferIndex = yy_buffer_index;
                tokenPosition.YyBufferStart = yy_buffer_start;
                tokenPosition.YyBufferEnd = yy_buffer_end;
                tokenPosition.YyLexicalState = yy_lexical_state;
                return tokenPosition;
            }
            set
            {
                var tokenPosition = (TokenPosition) value;
                myCurrentTokenType = tokenPosition.CurrentTokenType;
                yy_buffer_index = tokenPosition.YyBufferIndex;
                yy_buffer_start = tokenPosition.YyBufferStart;
                yy_buffer_end = tokenPosition.YyBufferEnd;
                yy_lexical_state = tokenPosition.YyLexicalState;
            }
        }

        public TokenNodeType TokenType
        {
            get
            {
                LocateToken();
                return myCurrentTokenType;
            }
        }

        public int TokenStart
        {
            get
            {
                LocateToken();
                return yy_buffer_start;
            }
        }

        public int TokenEnd
        {
            get
            {
                LocateToken();
                return yy_buffer_end;
            }
        }

        public IBuffer Buffer => yy_buffer;

        public uint LexerStateEx => (uint) yy_lexical_state;

        // This value is used during incremental lexing to give 7 characters of context when resuming lexing.
        // TODO: Why 7, and is that the correct value?
        public int LexemIndent => 7;
        public int EOFPos => yy_eof_pos;

        private void LocateToken()
        {
            if (myCurrentTokenType == null)
            {
                myCurrentTokenType = _locateToken();
            }
        }
    }
}