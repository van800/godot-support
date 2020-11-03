using JetBrains.ProjectModel;
using JetBrains.ReSharper.Plugins.Godot.Tscn.ProjectModel;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TestFramework;
using NUnit.Framework;

namespace JetBrains.ReSharper.Plugins.Godot.Tests.Tscn.ProjectModel
{
    [TestFixture]
    public class TscnProjectFileTypeTests : BaseTest
    {
        [Test]
        public void ProjectFileTypeIsRegistered()
        {
            Assert.NotNull(TscnProjectFileType.Instance);

            var projectFileTypes = Shell.Instance.GetComponent<IProjectFileTypes>();
            Assert.NotNull(projectFileTypes.GetFileType(TscnProjectFileType.Name));
        }

        [TestCase(TscnProjectFileType.TEXT_RESOURCE_EXTENSION)]
        [TestCase(TscnProjectFileType.TEXT_SCENE_EXTENSION)]
        [TestCase(TscnProjectFileType.EXPORTED_SCENE_EXTENSION)]
        public void ProjectFileTypeFromExtension(string extension)
        {
            var projectFileExtensions = Shell.Instance.GetComponent<IProjectFileExtensions>();
            Assert.AreSame(TscnProjectFileType.Instance, projectFileExtensions.GetFileType(extension));
        }
    }
}