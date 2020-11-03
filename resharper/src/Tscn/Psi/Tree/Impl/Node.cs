using JetBrains.Annotations;
using JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.DeclaredElements;
using JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Parsing.TokenNodes;
using JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Parsing.TokenNodeTypes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;

namespace JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Tree.Impl
{
    internal partial class Node
    {
        private readonly CachedPsiValueWithOffsets<IDeclaredElement> myCachedDeclaredElement =
            new CachedPsiValueWithOffsets<IDeclaredElement>();
        
        public override IDeclaredElement DeclaredElement
        {
            get
            {
                if (GetNameElement() == null)
                    return null;
                return myCachedDeclaredElement.GetValue(this,
                    () => CreateDeclaration(DeclaredName));
            }
        }
        
        public override string DeclaredName => GetNameElement()?.GetText() ?? SharedImplUtil.MISSING_DECLARATION_NAME;
        
        public override void SetName(string name)
        {
            // TODO: What happens to the Godot project if this is actually called? Can we just not support this?
            using (WriteLockCookie.Create())
            {
                var nameElement = GetNameElement();
                if (nameElement == null)
                {
                    // TODO: Is this the proper way to handle this situation?
                    return;
                }
                
                var newNameLiteral = new TscnGenericTokenNode(TscnTokenNodeTypes.STRING_LITERAL, name);
                ModificationUtil.ReplaceChild(nameElement, newNameLiteral);
            }
        }

        public override TreeTextRange GetNameRange()
        {
            return GetNameElement()?.GetTreeTextRange() ?? TreeTextRange.InvalidRange;
        }

        [CanBeNull]
        private IVariantValue GetNameElement()
        {
            // TODO: An util class for all things related TreeNodeCollection<IKeyValuePair> and typical names
            // TODO: Performance considerations, is for(;;) worth it here?
            foreach (var value in Values)
            {
                if (value.Identifier?.GetText() == "name")
                {
                    return value.Value;
                }
            }

            return null;
        }
        
        private IDeclaredElement CreateDeclaration(string declaredName)
        {
            return new NodeDeclaredElement(declaredName, GetSourceFile(), GetTreeStartOffset().Offset);
        }
    }
}