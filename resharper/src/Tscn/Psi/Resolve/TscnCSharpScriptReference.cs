using JetBrains.Annotations;
using JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Tree;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Resolve;
using JetBrains.ReSharper.Psi.Resolve;

namespace JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Resolve
{
    public class TscnCSharpScriptReference : CheckedReferenceBase<IVariantLiteral>, IQualifier
    {
        public TscnCSharpScriptReference([NotNull] IVariantLiteral owner) : base(owner)
        {
        }

        public override ResolveResultWithInfo ResolveWithoutCache()
        {
            throw new System.NotImplementedException();
        }

        public override string GetName()
        {
            // TODO: Utility method
            var text = myOwner.Literal.GetText();
            throw new System.NotImplementedException();
        }

        public override ISymbolTable GetReferenceSymbolTable(bool useReferenceName)
        {
            throw new System.NotImplementedException();
        }

        public override TreeTextRange GetTreeTextRange()
        {
            throw new System.NotImplementedException();
        }

        public override IReference BindTo(IDeclaredElement element)
        {
            throw new System.NotImplementedException();
        }

        public override IReference BindTo(IDeclaredElement element, ISubstitution substitution)
        {
            throw new System.NotImplementedException();
        }

        public override IAccessContext GetAccessContext()
        {
            throw new System.NotImplementedException();
        }

        public override ISymbolFilter[] GetSymbolFilters()
        {
            throw new System.NotImplementedException();
        }

        public ISymbolTable GetSymbolTable(SymbolTableMode mode)
        {
            throw new System.NotImplementedException();
        }

        public QualifierKind GetKind()
        {
            throw new System.NotImplementedException();
        }

        public bool Resolved { get; }
    }
}