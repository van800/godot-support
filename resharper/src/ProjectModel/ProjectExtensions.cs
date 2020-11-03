using JetBrains.Annotations;
using JetBrains.ProjectModel;

namespace JetBrains.ReSharper.Plugins.Godot.ProjectModel
{
    public static class ProjectExtensions
    {
        // TODO: Add tests for this
        public static bool IsGodotProject([CanBeNull] this IProject project)
        {
            if (project == null || !project.IsValid())
                return false;

            // TODO: Implement properly. Look for a Godot reference, just like in frontend
            return true;
        }
    }
}