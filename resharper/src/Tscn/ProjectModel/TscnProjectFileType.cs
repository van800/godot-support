using JetBrains.Annotations;
using JetBrains.ProjectModel;

namespace JetBrains.ReSharper.Plugins.Godot.Tscn.ProjectModel
{
    [ProjectFileTypeDefinition(Name)]
    public class TscnProjectFileType : KnownProjectFileType
    {
        public new const string Name = "TSCN";
        public const string TEXT_SCENE_EXTENSION = ".tscn";
        public const string TEXT_RESOURCE_EXTENSION = ".tres";
        public const string EXPORTED_SCENE_EXTENSION = ".escn";
        
        [CanBeNull, UsedImplicitly]
        public new static TscnProjectFileType Instance { get; private set; }

        private TscnProjectFileType()
            : base(Name, "TSCN", new[]
            {
                TEXT_SCENE_EXTENSION,
                TEXT_RESOURCE_EXTENSION,
                EXPORTED_SCENE_EXTENSION
            })
        {
        }
    }
}