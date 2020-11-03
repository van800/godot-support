using JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Parsing.TokenNodes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.Text;

namespace JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Parsing.TokenNodeTypes
{
    internal abstract class TscnFixedLengthTokenNodeType : TscnGenericTokenNodeType
    {
        protected TscnFixedLengthTokenNodeType(string s, int index, string representation) : base(s, index, representation)
        {
        }
    }
}