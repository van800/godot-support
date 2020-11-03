using JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Parsing.TokenNodeTypes;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;

namespace JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Parsing.TokenNodes
{
    internal class TscnIdentifierTokenNode : TscnTokenNodeBase
    {
        private readonly string myText;

        public TscnIdentifierTokenNode(string text)
        {
            myText = text;
        }

        public override int GetTextLength() => myText.Length;
        public override string GetText() => myText;
        public override NodeType NodeType => TscnTokenNodeTypes.IDENTIFIER;

        public string Name => GetText();
    }
}