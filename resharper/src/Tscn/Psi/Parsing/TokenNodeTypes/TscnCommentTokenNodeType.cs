using JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Parsing.TokenNodes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.Text;
using JetBrains.Util;

namespace JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Parsing.TokenNodeTypes
{
    internal class TscnCommentTokenNodeType : TscnGenericTokenNodeType
    {
        public TscnCommentTokenNodeType(int index) : base("SINGLE_LINE_COMMENT", index, "; single line comment")
        {
        }

        public override LeafElementBase Create(IBuffer buffer, TreeOffset startOffset, TreeOffset endOffset)
        {
            return new TscnCommentTokenNode(buffer.GetText(new TextRange(startOffset.Offset, endOffset.Offset)));
        }
        
        public override bool IsFiltered => true;
        public override bool IsComment => true;
    }
}