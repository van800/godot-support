using JetBrains.ReSharper.Plugins.Godot.Tscn.Psi;
using JetBrains.ReSharper.Psi;
using JetBrains.TestFramework;
using NUnit.Framework;

namespace JetBrains.ReSharper.Plugins.Godot.Tests.Tscn.Psi
{
    [TestFixture]
    public class TscnLanguageTests : BaseTest
    {
        [Test]
        public void LanguageIsRegistered()
        {
            Assert.NotNull(TscnLanguage.Instance);
            Assert.NotNull(Languages.Instance.GetLanguageByName(TscnLanguage.Name));
        }
    }
}