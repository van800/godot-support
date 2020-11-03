using JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Parsing.TokenNodes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.Text;
using JetBrains.Util;

namespace JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Parsing.TokenNodeTypes
{
    internal class TscnGenericTokenNodeType : TscnTokenNodeTypeBase
    {
        public TscnGenericTokenNodeType(string s, int index, string representation) : base(s, index)
        {
            TokenRepresentation = representation;
        }

        public override LeafElementBase Create(IBuffer buffer, TreeOffset startOffset, TreeOffset endOffset)
        {
            return new TscnGenericTokenNode(this, buffer.GetText(new TextRange(startOffset.Offset, endOffset.Offset)));
        }

        public override string TokenRepresentation { get; }
    }
}