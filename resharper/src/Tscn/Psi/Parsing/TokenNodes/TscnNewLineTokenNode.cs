using JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Parsing.TokenNodeTypes;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Parsing.TokenNodes
{
    internal class TscnNewLineTokenNode : TscnTokenNodeBase, IWhitespaceNode
    {
        private readonly string myText;

        public TscnNewLineTokenNode(string text)
        {
            myText = text;
        }

        public override int GetTextLength() => myText.Length;

        public override string GetText() => myText;

        public override NodeType NodeType => TscnTokenNodeTypes.NEW_LINE;

        public bool IsNewLine => true;
    }
}