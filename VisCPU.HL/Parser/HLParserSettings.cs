using System.Collections.Generic;
using System.Linq;

namespace VisCPU.HL.Parser
{

    /// <summary>
    ///     XL Parser SettingsSystem and Symbol Mappings
    /// </summary>
    public class HLParserSettings
    {

        private readonly string m_AbstractModifier = "abstract";
        private readonly string m_AsKey = "as";
        private readonly string m_BaseKey = "base";
        private readonly string m_BreakKey = "break";
        private readonly string m_CatchKey = "catch";
        private readonly string m_ClassKey = "class";
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
        public Dictionary < string, HLTokenType > ReservedKeys =>
            new Dictionary < string, HLTokenType >
            {
                { m_IfKey, HLTokenType.OpIf },
                { m_ElseKey, HLTokenType.OpElse },
                { m_ForEachKey, HLTokenType.OpForEach },
                { m_ForKey, HLTokenType.OpFor },
                { m_InKey, HLTokenType.OpIn },
                { m_IsKey, HLTokenType.OpIs },
                { m_AsKey, HLTokenType.OpAs },
                { m_DoKey, HLTokenType.OpDo },
                { m_WhileKey, HLTokenType.OpWhile },
                { m_SwitchKey, HLTokenType.OpSwitch },
                { m_TryKey, HLTokenType.OpTry },
                { m_CatchKey, HLTokenType.OpCatch },
                { m_FinallyKey, HLTokenType.OpFinally },
                { m_UsingKey, HLTokenType.OpUsing },
                { m_NamespaceKey, HLTokenType.OpNamespace },
                { m_ClassKey, HLTokenType.OpClass },
                { m_ContinueKey, HLTokenType.OpContinue },
                { m_BreakKey, HLTokenType.OpBreak },
                { m_NewKey, HLTokenType.OpNew },
                { m_BaseKey, HLTokenType.OpBase },
                { m_ThisKey, HLTokenType.OpThis },
                { m_PublicModifier, HLTokenType.OpPublicMod },
                { m_PrivateModifier, HLTokenType.OpPrivateMod },
                { m_ProtectedModifier, HLTokenType.OpProtectedMod },
                { m_VirtualModifier, HLTokenType.OpVirtualMod },
                { m_AbstractModifier, HLTokenType.OpAbstractMod },
                { m_OverrideModifier, HLTokenType.OpOverrideMod },
                { m_StaticModifier, HLTokenType.OpStaticMod },
                { m_ConstModifier, HLTokenType.OpConstMod },
                { m_ReturnKey, HLTokenType.OpReturn },
                { m_VoidKey, HLTokenType.OpTypeVoid },
                { m_OperatorKey, HLTokenType.OpOperatorImpl }
            };

        /// <summary>
        ///     Valid Member Modifiers.
        /// </summary>
        public Dictionary < string, HLTokenType > MemberModifiers =>
            new Dictionary < string, HLTokenType >
            {
                { m_ConstModifier, HLTokenType.OpConstMod },
                { m_StaticModifier, HLTokenType.OpStaticMod },
                { m_PublicModifier, HLTokenType.OpPublicMod },
                { m_PrivateModifier, HLTokenType.OpPrivateMod }
            };

        /// <summary>
        ///     Valid Class Modifiers
        /// </summary>
        public Dictionary < string, HLTokenType > ClassModifiers =>
            new Dictionary < string, HLTokenType >
            {
                { m_PublicModifier, HLTokenType.OpPublicMod },
                { m_PrivateModifier, HLTokenType.OpPrivateMod },
                { m_ProtectedModifier, HLTokenType.OpProtectedMod },
                { m_AbstractModifier, HLTokenType.OpAbstractMod },
                { m_StaticModifier, HLTokenType.OpStaticMod }
            };

        /// <summary>
        ///     Reverse Reserved Symbols (XLangToken - char)
        /// </summary>
        public Dictionary < HLTokenType, char > ReverseReservedSymbols =>
            ReservedSymbols.ToDictionary( pair => pair.Value, pair => pair.Key );

        /// <summary>
        ///     Reserved Symbols (char -  XLangToken)
        /// </summary>
        public Dictionary < char, HLTokenType > ReservedSymbols =>
            new Dictionary < char, HLTokenType >
            {
                { m_OperatorNumSign, HLTokenType.OpNumSign },
                { m_OperatorBackSlash, HLTokenType.OpBackSlash },
                { m_OperatorSingleQuote, HLTokenType.OpSingleQuote },
                { m_OperatorDoubleQuote, HLTokenType.OpDoubleQuote },
                { m_OperatorBlockOpen, HLTokenType.OpBlockBracketOpen },
                { m_OperatorBlockClose, HLTokenType.OpBlockBracketClose },
                { m_OperatorBracketsOpen, HLTokenType.OpBracketOpen },
                { m_OperatorBracketsClose, HLTokenType.OpBracketClose },
                { m_OperatorIndexAccessorOpen, HLTokenType.OpIndexerBracketOpen },
                { m_OperatorIndexAccessorClose, HLTokenType.OpIndexerBracketClose },
                { m_OperatorAsterisk, HLTokenType.OpAsterisk },
                { m_OperatorFwdSlash, HLTokenType.OpFwdSlash },
                { m_OperatorSemicolon, HLTokenType.OpSemicolon },
                { m_OperatorComma, HLTokenType.OpComma },
                { m_OperatorColon, HLTokenType.OpColon },
                { m_OperatorDot, HLTokenType.OpDot },
                { m_OperatorPlus, HLTokenType.OpPlus },
                { m_OperatorMinus, HLTokenType.OpMinus },
                { m_OperatorPercent, HLTokenType.OpPercent },
                { m_OperatorEquality, HLTokenType.OpEquality },
                { m_OperatorAnd, HLTokenType.OpAnd },
                { m_OperatorPipe, HLTokenType.OpPipe },
                { m_OperatorCap, HLTokenType.OpCap },
                { m_OperatorBang, HLTokenType.OpBang },
                { m_OperatorLessThan, HLTokenType.OpLessThan },
                { m_OperatorGreaterThan, HLTokenType.OpGreaterThan },
                { m_OperatorTilde, HLTokenType.OpTilde }
            };

    }

}
