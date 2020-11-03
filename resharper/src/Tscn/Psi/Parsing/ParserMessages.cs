using JetBrains.Util;

 // ReSharper disable InconsistentNaming

namespace JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Parsing
{
    public static class ParserMessages
    {
        public const string IDS_UNEXPECTED_TOKEN = "Unexpected token";
        public const string IDS_EXPECTED_SYMBOL = "{0} expected";
        public const string IDS_EXPECTED_SYMBOLS = "{0} or {1} expected";
        public const string IDS_TSCN_FILE = "tscn file";
        public const string IDS_VARIANT_LITERAL = "variant literal";
        public const string IDS_VARIANT_VALUE = "variant value";
        public const string IDS_RESOURCE_DESCRIPTOR = "resource file descriptor";
        public const string IDS_SCENE_DESCRIPTOR = "scene file descriptor";
        
        public static string GetString(string id) => id;
        
        public static string GetUnexpectedTokenMessage() => IDS_UNEXPECTED_TOKEN;
        
        public static string GetExpectedMessage(string tokenRepr)
        {
            return string.Format(GetString(IDS_EXPECTED_SYMBOL), tokenRepr).Capitalize();
        }
        
        public static string GetExpectedMessage(string firstExpectedSymbol, string secondExpectedSymbol)
        {
            return string.Format(GetString(IDS_EXPECTED_SYMBOLS), firstExpectedSymbol, secondExpectedSymbol).Capitalize();
        }
    }
}