using JetBrains.DataFlow;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Plugins.Godot.ProjectModel;
using JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Tree;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;

namespace JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Resolve
{
    [ReferenceProviderFactory]
    public class TscnSignalReferenceProviderFactory : IReferenceProviderFactory
    {
        public TscnSignalReferenceProviderFactory(Lifetime lifetime)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Changed = new Signal<IReferenceProviderFactory>(lifetime, GetType().FullName);
        }
        
        public IReferenceFactory CreateFactory(IPsiSourceFile sourceFile, IFile file, IWordIndex wordIndexForChecks)
        {
            var project = sourceFile.GetProject();
            if (project == null || !project.IsGodotProject())
                return null;

            if (sourceFile.PrimaryPsiLanguage.Is<TscnLanguage>())
            {
                return new TscnSignalReferenceFactory();
            }

            return null;
        }

        public ISignal<IReferenceProviderFactory> Changed { get; }
    }
}