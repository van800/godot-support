using System.Collections.Generic;
using JetBrains.ReSharper.Plugins.Godot.Tscn.ProjectModel;
using JetBrains.ReSharper.Plugins.Godot.Tscn.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

namespace JetBrains.ReSharper.Plugins.Godot.Tests.Tscn.Psi.Parsing
{
    [TestFileExtension(TscnProjectFileType.TEXT_RESOURCE_EXTENSION)]
    public class TresParserTests : ParserTestBase<TscnLanguage>
    {
        protected override string RelativeTestDataPath => @"Tscn\Psi\Parsing";

        [TestCase("full_resource")]
        public void TestParser(string name) => DoOneTest(name);

        protected override IList<string> TraceCategories()
        {
            return new List<string>() {"JetBrains.Application"};
        }
    }
}