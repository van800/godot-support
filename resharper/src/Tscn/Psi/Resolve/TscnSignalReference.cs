using System;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Tree;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Resolve;
using JetBrains.ReSharper.Psi.Impl.Shared.References;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Resx.Utils;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util;

namespace JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Resolve
{
    public class TscnSignalReference : QualifiableReferenceWithinElement<IVariantLiteral, ITokenNode>
    {
        private readonly IVariantLiteral myScriptPath;
        private readonly ISymbolCache mySymbolCache;

        public TscnSignalReference([NotNull] IVariantLiteral owner, [NotNull] IVariantLiteral scriptPath,
            [CanBeNull] IQualifier qualifier, ITokenNode token, TreeTextRange rangeWithin, ISymbolCache symbolCache)
            : base(owner, qualifier, token, rangeWithin)
        {
            myScriptPath = scriptPath;
            mySymbolCache = symbolCache;
        }

        // TODO: Verify signals only use instance methods
        public override Staticness GetStaticness() => Staticness.OnlyInstance;

        // No types are specified by signals
        public override ITypeElement GetQualifierTypeElement() => null;

        protected override IReference BindToInternal(IDeclaredElement declaredElement, ISubstitution substitution)
        {
            throw new NotImplementedException();
        }
        
        public override ISymbolTable GetReferenceSymbolTable(bool useReferenceName)
        {
            if (myQualifier == null)
            {
                var name = GetName();
                var symbolScope = mySymbolCache.GetSymbolScope(LibrarySymbolScope.FULL, true);
                var declaredElements = symbolScope.GetElementsByShortName(name);
                var symbolTable = ResolveUtil.CreateSymbolTable(declaredElements, 0);

                // GetElementsByShortName is case insensitive, so filter by exact name, which is case sensitive.
                return symbolTable.Filter(new ExactNameFilter(name));
            }
            return base.GetReferenceSymbolTable(useReferenceName);
        }
    }
}