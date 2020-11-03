using JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Parsing.TokenNodes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.Text;
using JetBrains.Util;

namespace JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Parsing.TokenNodeTypes
{
    internal class TscnWhitespaceTokenNodeType : TscnTokenNodeTypeBase
    {
        public TscnWhitespaceTokenNodeType(int index) : base("WHITESPACE", index)
        {
        }

        public override LeafElementBase Create(IBuffer buffer, TreeOffset startOffset, TreeOffset endOffset)
        {
            
            return new TscnWhitespaceTokenNode(buffer.GetText(new TextRange(startOffset.Offset, endOffset.Offset)));
        }

        public override bool IsWhitespace => true;
        public override string TokenRepresentation => " ";
    }
}