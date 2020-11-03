using JetBrains.ReSharper.Psi;

namespace JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.DeclaredElements
{
    public class NodeDeclaredElement : TscnDeclaredElementBase
    {
        public NodeDeclaredElement(string shortName, IPsiSourceFile sourceFile, int treeOffset)
            : base(shortName, sourceFile, treeOffset)
        {
        }

        public override DeclaredElementType GetElementType() => TscnDeclaredElementType.Node;
    }
}