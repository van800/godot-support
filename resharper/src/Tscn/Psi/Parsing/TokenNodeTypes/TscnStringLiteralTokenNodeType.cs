using JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Parsing.TokenNodes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.Text;
using JetBrains.Util;

namespace JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Parsing.TokenNodeTypes
{
    internal class TscnStringLiteralTokenNodeType : TscnTokenNodeTypeBase
    {
        public TscnStringLiteralTokenNodeType(int index) : base("STRING_LITERAL", index)
        {
        }

        public override LeafElementBase Create(IBuffer buffer, TreeOffset startOffset, TreeOffset endOffset)
        {
            return new TscnGenericTokenNode(this, buffer.GetText(new TextRange(startOffset.Offset, endOffset.Offset)));
        }

        public override bool IsStringLiteral => true;
        public override string TokenRepresentation { get; } = "\"string\"";
    }
}