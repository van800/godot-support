using JetBrains.ProjectModel;
using JetBrains.ReSharper.Plugins.Godot.Tscn.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.Text;
using JetBrains.UI.Icons;

namespace JetBrains.ReSharper.Plugins.Godot.Tscn.Psi
{
    [ProjectFileType(typeof(TscnProjectFileType))]
    public class TscnProjectFileLanguageService : ProjectFileLanguageService
    {
        protected override PsiLanguageType PsiLanguageType =>
            (PsiLanguageType) TscnLanguage.Instance ?? UnknownLanguage.Instance;

        // TODO: Add an icon
        public override IconId Icon => null;
        
        // TODO: Check this works as expected.
        public TscnProjectFileLanguageService() : base(TscnProjectFileType.Instance)
        {
        }

        public override ILexerFactory GetMixedLexerFactory(ISolution solution, IBuffer buffer, IPsiSourceFile sourceFile = null)
        {
            var languageService = TscnLanguage.Instance.LanguageService();
            return languageService?.GetPrimaryLexerFactory();
        }
    }
}