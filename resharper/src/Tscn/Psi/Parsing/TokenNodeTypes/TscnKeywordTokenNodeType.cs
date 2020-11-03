namespace JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Parsing.TokenNodeTypes
{
    internal class TscnKeywordTokenNodeType : TscnFixedLengthTokenNodeType
    {
        protected TscnKeywordTokenNodeType(string s, int index, string representation) : base(s, index, representation)
        {
        }

        public override bool IsKeyword => true;
    }
}