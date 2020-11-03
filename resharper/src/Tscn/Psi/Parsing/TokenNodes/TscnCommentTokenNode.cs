using JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Parsing.TokenNodeTypes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Parsing.TokenNodes
{
    public class TscnCommentTokenNode : TscnTokenNodeBase, ICommentNode
    {
        private readonly string myText;

        public TscnCommentTokenNode(string text)
        {
            myText = text;
        }

        public override NodeType NodeType => TscnTokenNodeTypes.COMMENT;

        public override int GetTextLength() => myText.Length;
        public override string GetText() => myText;
        public override bool IsFiltered() => true;
        
        public TreeTextRange GetCommentRange()
        {
            // Remove the semicolon
            var start = GetTreeStartOffset();
            return new TreeTextRange(start + 1, start + GetTextLength());
        }
        
        public string CommentText => myText.Substring(1);
    }
}