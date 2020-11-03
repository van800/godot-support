using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TestFramework;
using JetBrains.TestFramework.Application.Zones;
using NUnit.Framework;

[assembly: RequiresThread(System.Threading.ApartmentState.STA)]

namespace JetBrains.ReSharper.Plugins.Godot.Tests
{
    [ZoneDefinition]
    public interface IGodotTestZone : ITestsEnvZone, IRequire<PsiFeatureTestZone>
    {
    }

    [SetUpFixture]
    public class TestEnvironment : ExtensionTestEnvironmentAssembly<IGodotTestZone>
    {
    }
}