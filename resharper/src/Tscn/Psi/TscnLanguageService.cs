using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Parsing;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Caches2;
using JetBrains.ReSharper.Psi.Impl;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Text;
using JetBrains.Util;

namespace JetBrains.ReSharper.Plugins.Godot.Tscn.Psi
{
    [Language(typeof(TscnLanguage))]
    public class TscnLanguageService : LanguageService
    {
        public TscnLanguageService([NotNull] PsiLanguageType psiLanguageType, [NotNull] IConstantValueService constantValueService) : base(psiLanguageType, constantValueService)
        {
        }

        public override ILexerFactory GetPrimaryLexerFactory()
        {
            return new TscnLexerFactory();
        }

        public override ILexer CreateFilteringLexer(ILexer lexer)
        {
            return new TscnFilteringLexer(lexer);
        }

        public override IParser CreateParser(ILexer lexer, IPsiModule module, IPsiSourceFile sourceFile)
        {
            return new TscnParser(lexer as ILexer<int> ?? lexer.ToCachingLexer());
        }

        public override IEnumerable<ITypeDeclaration> FindTypeDeclarations(IFile file)
        {
            return EmptyList<ITypeDeclaration>.Enumerable;
        }

        // TODO: Do we need this?
        public override ILanguageCacheProvider CacheProvider => null;
        public override bool IsCaseSensitive => true;
        public override bool SupportTypeMemberCache => false;
        public override ITypePresenter TypePresenter => DefaultTypePresenter.Instance;

        private class TscnLexerFactory : ILexerFactory
        {
            public ILexer CreateLexer(IBuffer buffer)
            {
                return new TscnLexerGenerated(buffer);
            }
        }
    }
}