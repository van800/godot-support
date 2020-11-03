using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;

namespace JetBrains.ReSharper.Plugins.Godot.Tscn.Psi
{
    /// <summary>
    /// The language used by Godot for text scene files and resources in general.
    /// </summary>
    [LanguageDefinition(Name)]
    public class TscnLanguage : KnownLanguage
    {
        public new const string Name = "TSCN";

        [CanBeNull, UsedImplicitly]
        public static TscnLanguage Instance { get; private set; }
        
        private TscnLanguage() : base(Name, "TSCN")
        {
        }

        protected TscnLanguage([NotNull] string name) : base(name)
        {
        }

        protected TscnLanguage([NotNull] string name, [NotNull] string presentableName) : base(name, presentableName)
        {
        }
    }
}