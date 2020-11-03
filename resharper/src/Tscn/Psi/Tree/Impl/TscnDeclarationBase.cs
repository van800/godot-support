using System.Xml;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Tree.Impl
{
    public abstract class TscnDeclarationBase : TscnCompositeElement, IDeclaration
    {
        public abstract IDeclaredElement DeclaredElement { get; }
        
        public abstract string DeclaredName { get; }
        
        public abstract void SetName(string name);
        
        public abstract TreeTextRange GetNameRange();

        public bool IsSynthetic() => false;
        
        public XmlNode GetXMLDoc(bool inherit) => null;
    }
}