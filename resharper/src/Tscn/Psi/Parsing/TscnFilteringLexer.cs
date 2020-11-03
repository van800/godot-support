using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Parsing;

namespace JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Parsing
{
    public class TscnFilteringLexer : FilteringLexer, ILexer<int>
    {
        public TscnFilteringLexer([NotNull] ILexer lexer) : base(lexer)
        {
        }

        protected override bool Skip(TokenNodeType tokenType)
        {
            if (tokenType.IsWhitespace || tokenType.IsComment || tokenType.IsFiltered)
                return true;

            return false;
        }


        int ILexer<int>.CurrentPosition
        {
            get => ((ILexer<int>) myLexer).CurrentPosition;
            set => ((ILexer<int>) myLexer).CurrentPosition = value;
        }
    }
}