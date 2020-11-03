using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;

namespace JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Tree
{
    public abstract class TscnCompositeNodeType : CompositeNodeType
    {
        protected TscnCompositeNodeType(string s, int index)
            : base(s, index)
        {
        }
    }
}