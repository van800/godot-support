using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Parsing;

namespace JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Parsing.TokenNodes
{
    internal class TscnGenericTokenNode : TscnTokenNodeBase
    {
        private readonly TokenNodeType myTokenNodeType;
        private readonly string myText;

        public TscnGenericTokenNode(TokenNodeType tokenNodeType, string text)
        {
            myTokenNodeType = tokenNodeType;
            myText = text;
        }

        public override NodeType NodeType => myTokenNodeType;
        public override int GetTextLength() => myText.Length;
        public override string GetText() => myText;
    }
}