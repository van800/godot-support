using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.Text;

namespace JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Parsing.TokenNodeTypes
{
    internal abstract class TscnTokenNodeTypeBase : TokenNodeType
    {
        protected TscnTokenNodeTypeBase(string s, int index) : base(s, index)
        {
        }

        public override bool IsWhitespace => false;
        public override bool IsComment => false;
        public override bool IsStringLiteral => false;
        public override bool IsConstantLiteral => false;
        public override bool IsIdentifier => false;
        public override bool IsKeyword => false;
        public virtual bool IsContextualKeyword => false;
    }
}