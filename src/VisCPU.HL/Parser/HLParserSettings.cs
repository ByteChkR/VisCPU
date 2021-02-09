using System.Collections.Generic;
using System.Linq;

namespace VisCPU.HL.Parser
{

    /// <summary>
    ///     XL Parser SettingsSystem and Symbol Mappings
    /// </summary>
    public class HlParserSettings
    {

        private readonly string m_AbstractModifier = "abstract";
        private readonly string m_AsKey = "as";
        private readonly string m_BaseKey = "base";
        private readonly string m_BreakKey = "break";
        private readonly string m_CatchKey = "catch";
        private readonly string m_ClassKey = "class";
        private readonly string m_StructKey = "struct";
        private readonly string m_ContinueKey = "continue";
        private readonly string m_DoKey = "do";
        private readonly string m_ElseKey = "else";
        private readonly string m_FinallyKey = "finally";
        private readonly string m_ForEachKey = "foreach";
        private readonly string m_ForKey = "for";
        private readonly string m_IfKey = "if";
        private readonly string m_InKey = "in";
        private readonly string m_IsKey = "is";
        private readonly string m_NamespaceKey = "namespace";
        private readonly string m_NewKey = "new";
        private readonly string m_DeleteKey = "delete";
        private readonly char m_OperatorAnd = '&';
        private readonly char m_OperatorAsterisk = '*';
        private readonly char m_OperatorBackSlash = '\\';
        private readonly char m_OperatorBang = '!';
        private readonly char m_OperatorBlockClose = '}';
        private readonly char m_OperatorBlockOpen = '{';
        private readonly char m_OperatorBracketsClose = ')';
        private readonly char m_OperatorBracketsOpen = '(';
        private readonly char m_OperatorCap = '^';
        private readonly char m_OperatorColon = ':';
        private readonly char m_OperatorComma = ',';
        private readonly char m_OperatorDot = '.';

        private readonly char m_OperatorDoubleQuote = '"';
        private readonly char m_OperatorEquality = '=';
        private readonly char m_OperatorFwdSlash = '/';
        private readonly char m_OperatorGreaterThan = '>';
        private readonly char m_OperatorIndexAccessorClose = ']';
        private readonly char m_OperatorIndexAccessorOpen = '[';
        private readonly string m_OperatorKey = "operator";
        private readonly char m_OperatorLessThan = '<';
        private readonly char m_OperatorMinus = '-';
        private readonly char m_OperatorNumSign = '#';
        private readonly char m_OperatorPercent = '%';
        private readonly char m_OperatorPipe = '|';
        private readonly char m_OperatorPlus = '+';
        private readonly char m_OperatorSemicolon = ';';
        private readonly char m_OperatorSingleQuote = '\'';
        private readonly char m_OperatorTilde = '~';
        private readonly string m_OverrideModifier = "override";
        private readonly string m_PrivateModifier = "private";
        private readonly string m_ProtectedModifier = "protected";

        private readonly string m_PublicModifier = "public";
        private readonly string m_ReturnKey = "return";
        private readonly string m_StaticModifier = "static";
        private readonly string m_ConstModifier = "const";
        private readonly string m_SwitchKey = "switch";
        private readonly string m_ThisKey = "this";
        private readonly string m_TryKey = "try";
        private readonly string m_UsingKey = "using";
        private readonly string m_VirtualModifier = "virtual";
        private readonly string m_VoidKey = "void";
        private readonly string m_WhileKey = "while";

        /// <summary>
        ///     Reserved Key Map
        /// </summary>
        public Dictionary < string, HlTokenType > ReservedKeys =>
            new Dictionary < string, HlTokenType >
            {
                { m_IfKey, HlTokenType.OpIf },
                { m_ElseKey, HlTokenType.OpElse },
                { m_ForEachKey, HlTokenType.OpForEach },
                { m_ForKey, HlTokenType.OpFor },
                { m_InKey, HlTokenType.OpIn },
                { m_IsKey, HlTokenType.OpIs },
                { m_AsKey, HlTokenType.OpAs },
                { m_DoKey, HlTokenType.OpDo },
                { m_WhileKey, HlTokenType.OpWhile },
                { m_SwitchKey, HlTokenType.OpSwitch },
                { m_TryKey, HlTokenType.OpTry },
                { m_CatchKey, HlTokenType.OpCatch },
                { m_FinallyKey, HlTokenType.OpFinally },
                { m_UsingKey, HlTokenType.OpUsing },
                { m_NamespaceKey, HlTokenType.OpNamespace },
                { m_ClassKey, HlTokenType.OpClass },
                { m_StructKey, HlTokenType.OpClass },
                { m_ContinueKey, HlTokenType.OpContinue },
                { m_BreakKey, HlTokenType.OpBreak },
                { m_NewKey, HlTokenType.OpNew },
                { m_DeleteKey, HlTokenType.OpDelete },
                { m_BaseKey, HlTokenType.OpBase },
                { m_ThisKey, HlTokenType.OpThis },
                { m_PublicModifier, HlTokenType.OpPublicMod },
                { m_PrivateModifier, HlTokenType.OpPrivateMod },
                { m_ProtectedModifier, HlTokenType.OpProtectedMod },
                { m_VirtualModifier, HlTokenType.OpVirtualMod },
                { m_AbstractModifier, HlTokenType.OpAbstractMod },
                { m_OverrideModifier, HlTokenType.OpOverrideMod },
                { m_StaticModifier, HlTokenType.OpStaticMod },
                { m_ConstModifier, HlTokenType.OpConstMod },
                { m_ReturnKey, HlTokenType.OpReturn },
                { m_VoidKey, HlTokenType.OpTypeVoid },
                { m_OperatorKey, HlTokenType.OpOperatorImpl }
            };

        /// <summary>
        ///     Valid Member Modifiers.
        /// </summary>
        public Dictionary < string, HlTokenType > MemberModifiers =>
            new Dictionary < string, HlTokenType >
            {
                { m_ConstModifier, HlTokenType.OpConstMod },
                { m_StaticModifier, HlTokenType.OpStaticMod },
                { m_PublicModifier, HlTokenType.OpPublicMod },
                { m_AbstractModifier, HlTokenType.OpAbstractMod },
                { m_VirtualModifier, HlTokenType.OpVirtualMod },
                { m_OverrideModifier, HlTokenType.OpOverrideMod },
                { m_PrivateModifier, HlTokenType.OpPrivateMod }
            };

        /// <summary>
        ///     Valid Class Modifiers
        /// </summary>
        public Dictionary < string, HlTokenType > ClassModifiers =>
            new Dictionary < string, HlTokenType >
            {
                { m_PublicModifier, HlTokenType.OpPublicMod },
                { m_PrivateModifier, HlTokenType.OpPrivateMod },
                { m_ProtectedModifier, HlTokenType.OpProtectedMod },
                { m_AbstractModifier, HlTokenType.OpAbstractMod },
                { m_StaticModifier, HlTokenType.OpStaticMod }
            };

        /// <summary>
        ///     Reverse Reserved Symbols (XLangToken - char)
        /// </summary>
        public Dictionary < HlTokenType, char > ReverseReservedSymbols =>
            ReservedSymbols.ToDictionary( pair => pair.Value, pair => pair.Key );

        /// <summary>
        ///     Reserved Symbols (char -  XLangToken)
        /// </summary>
        public Dictionary < char, HlTokenType > ReservedSymbols =>
            new Dictionary < char, HlTokenType >
            {
                { m_OperatorNumSign, HlTokenType.OpNumSign },
                { m_OperatorBackSlash, HlTokenType.OpBackSlash },
                { m_OperatorSingleQuote, HlTokenType.OpSingleQuote },
                { m_OperatorDoubleQuote, HlTokenType.OpDoubleQuote },
                { m_OperatorBlockOpen, HlTokenType.OpBlockBracketOpen },
                { m_OperatorBlockClose, HlTokenType.OpBlockBracketClose },
                { m_OperatorBracketsOpen, HlTokenType.OpBracketOpen },
                { m_OperatorBracketsClose, HlTokenType.OpBracketClose },
                { m_OperatorIndexAccessorOpen, HlTokenType.OpIndexerBracketOpen },
                { m_OperatorIndexAccessorClose, HlTokenType.OpIndexerBracketClose },
                { m_OperatorAsterisk, HlTokenType.OpAsterisk },
                { m_OperatorFwdSlash, HlTokenType.OpFwdSlash },
                { m_OperatorSemicolon, HlTokenType.OpSemicolon },
                { m_OperatorComma, HlTokenType.OpComma },
                { m_OperatorColon, HlTokenType.OpColon },
                { m_OperatorDot, HlTokenType.OpDot },
                { m_OperatorPlus, HlTokenType.OpPlus },
                { m_OperatorMinus, HlTokenType.OpMinus },
                { m_OperatorPercent, HlTokenType.OpPercent },
                { m_OperatorEquality, HlTokenType.OpEquality },
                { m_OperatorAnd, HlTokenType.OpAnd },
                { m_OperatorPipe, HlTokenType.OpPipe },
                { m_OperatorCap, HlTokenType.OpCap },
                { m_OperatorBang, HlTokenType.OpBang },
                { m_OperatorLessThan, HlTokenType.OpLessThan },
                { m_OperatorGreaterThan, HlTokenType.OpGreaterThan },
                { m_OperatorTilde, HlTokenType.OpTilde }
            };

    }

}
