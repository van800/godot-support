using System.IO;
using JetBrains.ReSharper.Plugins.Godot.Tscn.ProjectModel;
using JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Parsing;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.TestFramework;
using JetBrains.Text;
using NUnit.Framework;

namespace JetBrains.ReSharper.Plugins.Godot.Tests.Tscn.Psi.Parsing
{
    //[TestUnity]
    [TestFileExtension(TscnProjectFileType.TEXT_SCENE_EXTENSION)]
    public class LexerTests : LexerTestBase
    {
        protected override string RelativeTestDataPath => @"Tscn\Psi\Lexing";

        protected override ILexer CreateLexer(IBuffer buffer)
        {
            var text = buffer.GetText();
            text = NormaliseLindEndings(text);
            return new TscnLexerGenerated(new StringBuffer(text));
        }

        private string NormaliseLindEndings(string text)
        {
            // TeamCity doesn't respect .gitattributes and pulls everything out as
            // LF, instead of CRLF. Normalise to CRLF
            return !text.Contains("\r\n") ? text.Replace("\n", "\r\n") : text;
        }

        protected override void WriteToken(TextWriter writer, ILexer lexer)
        {
            string str1 = lexer.GetTokenText().Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t");
            string str2 = $"{lexer.TokenStart:D4}: {lexer.TokenType} '{str1}'";
            writer.WriteLine(str2);
        }

        [TestCase("full_scene")]
        [TestCase("comment")]
        [TestCase("string")]
        [TestCase("integer_literal")]
        [TestCase("real_literal")]
        [TestCase("identifier")]
        public void TestLexer(string name) => DoOneTest(name);
    }
}