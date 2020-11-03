using System.Collections.Generic;
using System.Xml;
using JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Tree;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using JetBrains.Util.DataStructures;

namespace JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.DeclaredElements
{
    public abstract class TscnDeclaredElementBase : ITscnDeclaredElement
    {
        private readonly IPsiSourceFile mySourceFile;
        
        protected TscnDeclaredElementBase(string shortName, IPsiSourceFile sourceFile, int treeOffset)
        {
            mySourceFile = sourceFile;
            TreeOffset = treeOffset;
            ShortName = shortName;
        }

        protected int TreeOffset { get; }
        
        public IPsiServices GetPsiServices()
        {
            return mySourceFile.GetPsiServices();
        }

        public IList<IDeclaration> GetDeclarations()
        {
            // TODO: This is almost certainly not what we want, taken from ShaderLab
            if (!(mySourceFile.GetPrimaryPsiFile() is ITscnFile psi))
                return EmptyList<IDeclaration>.InstanceList;

            var node = psi.FindNodeAt(TreeTextRange.FromLength(new TreeOffset(TreeOffset), 1));
            while (node != null && !(node is IDeclaration))
                node = node.Parent;
            if (node == null)
                return EmptyList<IDeclaration>.Instance;

            return new[] {(IDeclaration) node};
        }

        public IList<IDeclaration> GetDeclarationsIn(IPsiSourceFile sourceFile)
        {
            if (mySourceFile == sourceFile)
                return GetDeclarations();
            return EmptyList<IDeclaration>.Instance;
        }

        public abstract DeclaredElementType GetElementType();

        public XmlNode GetXMLDoc(bool inherit) => null;

        public XmlNode GetXMLDescriptionSummary(bool inherit) => null;

        public bool IsValid() => true;

        public bool IsSynthetic() => false;

        public HybridCollection<IPsiSourceFile> GetSourceFiles()
        {
            return new HybridCollection<IPsiSourceFile>(mySourceFile);
        }

        public bool HasDeclarationsIn(IPsiSourceFile sourceFile)
        {
            return mySourceFile == sourceFile;
        }

        public string ShortName { get; }
        
        public bool CaseSensitiveName => true;
        
        // ReSharper disable once AssignNullToNotNullAttribute
        public PsiLanguageType PresentationLanguage => TscnLanguage.Instance;
    }
}