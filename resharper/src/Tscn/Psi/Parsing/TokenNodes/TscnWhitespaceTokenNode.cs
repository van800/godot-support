using JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Parsing.TokenNodeTypes;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Parsing.TokenNodes
{
    public class TscnWhitespaceTokenNode : TscnTokenNodeBase, IWhitespaceNode
    {
        private readonly string myText;

        public TscnWhitespaceTokenNode(string text)
        {
            myText = text;
        }

        public override int GetTextLength() => myText.Length;

        public override string GetText() => myText;

        public override NodeType NodeType => TscnTokenNodeTypes.WHITE_SPACE;

        public bool IsNewLine => false;
    }
}