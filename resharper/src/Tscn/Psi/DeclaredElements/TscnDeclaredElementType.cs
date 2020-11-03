using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.UI.Icons;

namespace JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.DeclaredElements
{
    public class TscnDeclaredElementType : DeclaredElementType
    {
        // TODO: Icon
        public static readonly TscnDeclaredElementType Node = new TscnDeclaredElementType("SceneNode", null);
            
        private readonly IconId myIconId;

        protected TscnDeclaredElementType([NotNull] string name, IconId iconId) : base(name)
        {
            PresentableName = name;
            myIconId = iconId;
        }

        public override IconId GetImage() => myIconId;

        public override bool IsPresentable(PsiLanguageType language) => language.Is<TscnLanguage>();

        public override string PresentableName { get; }
        protected override IDeclaredElementPresenter DefaultPresenter => TscnDeclaredElementPresenter.Instance;
    }
}