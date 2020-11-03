using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Gen;
using JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Parsing.TokenNodeTypes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;

namespace JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Parsing
{
    internal class TscnParser : TscnParserGenerated, ITscnParser
    {
        // TODO: Consider interning identifiers and literals
        public TscnParser([NotNull] ILexer<int> lexer)
        {
            // TODO: Add a MissingTokenInserter
            SetLexer(new TscnFilteringLexer(lexer));
        }

        public IFile ParseFile()
        {
            var element = ParseTscnFile();
            return (IFile) element;
        }

        private TreeElement CreateToken(TokenNodeType tokenType)
        {
            Assertion.AssertNotNull(tokenType, "tokenType != null");

            var element = tokenType.Create(myLexer.Buffer, new TreeOffset(myLexer.TokenStart),
                new TreeOffset(myLexer.TokenEnd));

            SetOffset(element, myLexer.TokenStart);
            myLexer.Advance();
            return element;
        }

        private TreeElement MatchContextualKeyword(TokenNodeType tokenType)
        {
            if (ExpectContextualKeyword(tokenType))
            {
                return CreateToken(tokenType);
            }

            throw new UnexpectedToken(ParserMessages.GetExpectedMessage(
                $"Contextual keyword '{tokenType.TokenRepresentation}'"));
        }

        private bool ExpectContextualKeyword(TokenNodeType type)
        {
            if (myLexer.TokenType != TscnTokenNodeTypes.IDENTIFIER) return false;

            return myLexer.CompareTokenText(type.TokenRepresentation);
    }

        protected override TreeElement MatchConnectionKeyword()
            => MatchContextualKeyword(TscnTokenNodeTypes.CONNECTION_KEYWORD);

        protected override bool ExpectConnectionKeyword()
            => ExpectContextualKeyword(TscnTokenNodeTypes.CONNECTION_KEYWORD);

        protected override TreeElement MatchExtResourceKeyword()
            => MatchContextualKeyword(TscnTokenNodeTypes.EXT_RESOURCE_KEYWORD);

        protected override bool ExpectExtResourceKeyword()
            => ExpectContextualKeyword(TscnTokenNodeTypes.EXT_RESOURCE_KEYWORD);

        protected override TreeElement MatchFormatKeyword()
            => MatchContextualKeyword(TscnTokenNodeTypes.FORMAT_KEYWORD);

        protected override bool ExpectFormatKeyword()
            => ExpectContextualKeyword(TscnTokenNodeTypes.FORMAT_KEYWORD);

        protected override TreeElement MatchLoadStepsKeyword()
            => MatchContextualKeyword(TscnTokenNodeTypes.LOAD_STEPS_KEYWORD);

        protected override bool ExpectLoadStepsKeyword()
            => ExpectContextualKeyword(TscnTokenNodeTypes.LOAD_STEPS_KEYWORD);

        protected override TreeElement MatchMainResourceKeyword()
            => MatchContextualKeyword(TscnTokenNodeTypes.MAIN_RESOURCE_KEYWORD);

        protected override bool ExpectMainResourceKeyword()
            => ExpectContextualKeyword(TscnTokenNodeTypes.MAIN_RESOURCE_KEYWORD);

        protected override TreeElement MatchNodeKeyword()
            => MatchContextualKeyword(TscnTokenNodeTypes.NODE_KEYWORD);

        protected override bool ExpectNodeKeyword()
            => ExpectContextualKeyword(TscnTokenNodeTypes.NODE_KEYWORD);

        protected override TreeElement MatchResourceKeyword()
            => MatchContextualKeyword(TscnTokenNodeTypes.RESOURCE_KEYWORD);

        protected override bool ExpectResourceKeyword()
            => ExpectContextualKeyword(TscnTokenNodeTypes.RESOURCE_KEYWORD);

        protected override TreeElement MatchSceneKeyword()
            => MatchContextualKeyword(TscnTokenNodeTypes.SCENE_KEYWORD);

        protected override bool ExpectSceneKeyword()
            => ExpectContextualKeyword(TscnTokenNodeTypes.SCENE_KEYWORD);

        protected override TreeElement MatchSubResourceKeyword()
            => MatchContextualKeyword(TscnTokenNodeTypes.SUB_RESOURCE_KEYWORD);

        protected override bool ExpectSubResourceKeyword()
            => ExpectContextualKeyword(TscnTokenNodeTypes.SUB_RESOURCE_KEYWORD);

        protected override TreeElement MatchTypeKeyword()
            => MatchContextualKeyword(TscnTokenNodeTypes.TYPE_KEYWORD);

        protected override bool ExpectTypeKeyword()
            => ExpectContextualKeyword(TscnTokenNodeTypes.TYPE_KEYWORD);

        protected override TreeElement MatchFalseKeyword()
            => MatchContextualKeyword(TscnTokenNodeTypes.FALSE_KEYWORD);

        protected override bool ExpectFalseKeyword()
            => ExpectContextualKeyword(TscnTokenNodeTypes.FALSE_KEYWORD);

        protected override TreeElement MatchTrueKeyword()
            => MatchContextualKeyword(TscnTokenNodeTypes.TRUE_KEYWORD);

        protected override bool ExpectTrueKeyword()
            => ExpectContextualKeyword(TscnTokenNodeTypes.TRUE_KEYWORD);

        protected override TreeElement MatchNullKeyword()
            => MatchContextualKeyword(TscnTokenNodeTypes.NULL_KEYWORD);

        protected override bool ExpectNullKeyword()
            => ExpectContextualKeyword(TscnTokenNodeTypes.NULL_KEYWORD);

        protected override TreeElement MatchNilKeyword()
            => MatchContextualKeyword(TscnTokenNodeTypes.NIL_KEYWORD);

        protected override bool ExpectNilKeyword()
            => ExpectContextualKeyword(TscnTokenNodeTypes.NIL_KEYWORD);

        protected override TreeElement MatchInfKeyword()
            => MatchContextualKeyword(TscnTokenNodeTypes.INF_KEYWORD);

        protected override bool ExpectInfKeyword()
            => ExpectContextualKeyword(TscnTokenNodeTypes.INF_KEYWORD);

        protected override TreeElement MatchNaNKeyword()
            => MatchContextualKeyword(TscnTokenNodeTypes.NAN_KEYWORD);

        protected override bool ExpectNaNKeyword()
            => ExpectContextualKeyword(TscnTokenNodeTypes.NAN_KEYWORD);
    }
}