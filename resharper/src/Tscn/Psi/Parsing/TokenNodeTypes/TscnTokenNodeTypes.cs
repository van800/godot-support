using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;

namespace JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Parsing.TokenNodeTypes
{
    public partial class TscnTokenNodeTypes
    {
        public static readonly TokenNodeType BAD_CHARACTER = new TscnGenericTokenNodeType("BAD_CHARACTER", LAST_GENERATED_TOKEN_TYPE_INDEX + 1, "�");
        public static readonly TokenNodeType WHITE_SPACE = new TscnWhitespaceTokenNodeType(LAST_GENERATED_TOKEN_TYPE_INDEX + 2);
        public static readonly TokenNodeType NEW_LINE = new TscnNewLineTokenNodeType(LAST_GENERATED_TOKEN_TYPE_INDEX + 3);
        public const int IDENTIFIER_NODE_TYPE_INDEX = LAST_GENERATED_TOKEN_TYPE_INDEX + 4;
        public static readonly TokenNodeType IDENTIFIER = new TscnIdentifierTokenNodeType(IDENTIFIER_NODE_TYPE_INDEX);
        public static readonly TokenNodeType COMMENT = new TscnCommentTokenNodeType(LAST_GENERATED_TOKEN_TYPE_INDEX + 5);
        public static readonly TokenNodeType EOF = new TscnGenericTokenNodeType("EOF", LAST_GENERATED_TOKEN_TYPE_INDEX + 6, "EOF");
        public const int NUMERIC_LITERAL_NODE_TYPE_INDEX = LAST_GENERATED_TOKEN_TYPE_INDEX + 7;
        public static readonly TokenNodeType NUMERIC_LITERAL = new TscnNumericLiteralTokenNodeType(NUMERIC_LITERAL_NODE_TYPE_INDEX);
        public const int COLOR_LITERAL_NODE_TYPE_INDEX = LAST_GENERATED_TOKEN_TYPE_INDEX + 8;
        public static readonly TokenNodeType COLOR_LITERAL = new TscnColorLiteralTokenNodeType(COLOR_LITERAL_NODE_TYPE_INDEX);
        public const int STRING_LITERAL_NODE_TYPE_INDEX = LAST_GENERATED_TOKEN_TYPE_INDEX + 9;
        public static readonly TokenNodeType STRING_LITERAL = new TscnStringLiteralTokenNodeType(STRING_LITERAL_NODE_TYPE_INDEX);
        public const int STRING_NAME_LITERAL_NODE_TYPE_INDEX = LAST_GENERATED_TOKEN_TYPE_INDEX + 10;
        public static readonly TokenNodeType STRING_NAME_LITERAL = new TscnStringNameLiteralTokenNodeType(STRING_NAME_LITERAL_NODE_TYPE_INDEX);

        public static readonly NodeTypeSet KEYWORDS;
        public static readonly NodeTypeSet CONTEXTUAL_KEYWORDS;

        static TscnTokenNodeTypes()
        {
            KEYWORDS = new NodeTypeSet(
                SCENE_KEYWORD,
                RESOURCE_KEYWORD,
                EXT_RESOURCE_KEYWORD,
                SUB_RESOURCE_KEYWORD,
                MAIN_RESOURCE_KEYWORD,
                NODE_KEYWORD,
                CONNECTION_KEYWORD,
                FORMAT_KEYWORD,
                LOAD_STEPS_KEYWORD,
                TYPE_KEYWORD,
                FALSE_KEYWORD,
                TRUE_KEYWORD,
                NULL_KEYWORD,
                NIL_KEYWORD,
                INF_KEYWORD,
                NAN_KEYWORD
            );
            
            CONTEXTUAL_KEYWORDS = new NodeTypeSet(
                SCENE_KEYWORD,
                RESOURCE_KEYWORD,
                EXT_RESOURCE_KEYWORD,
                SUB_RESOURCE_KEYWORD,
                MAIN_RESOURCE_KEYWORD,
                NODE_KEYWORD,
                CONNECTION_KEYWORD,
                FORMAT_KEYWORD,
                LOAD_STEPS_KEYWORD,
                TYPE_KEYWORD,
                FALSE_KEYWORD,
                TRUE_KEYWORD,
                NULL_KEYWORD,
                NIL_KEYWORD,
                INF_KEYWORD,
                NAN_KEYWORD
            );
        }
    }
}