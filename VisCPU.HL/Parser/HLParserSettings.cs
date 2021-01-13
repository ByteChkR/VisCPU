using System.Collections.Generic;
using System.Linq;

namespace VisCPU.HL.Parser
{

    /// <summary>
    ///     XL Parser SettingsSystem and Symbol Mappings
    /// </summary>
    public class HLParserSettings
    {

        private readonly string AbstractModifier = "abstract";
        private readonly string AsKey = "as";
        private readonly string BaseKey = "base";
        private readonly string BreakKey = "break";
        private readonly string CatchKey = "catch";
        private readonly string ClassKey = "class";
        private readonly string ContinueKey = "continue";
        private readonly string DoKey = "do";
        private readonly string ElseKey = "else";
        private readonly string FinallyKey = "finally";
        private readonly string ForEachKey = "foreach";
        private readonly string ForKey = "for";
        private readonly string IfKey = "if";
        private readonly string InKey = "in";
        private readonly string IsKey = "is";
        private readonly string NamespaceKey = "namespace";
        private readonly string NewKey = "new";
        private readonly char OperatorAnd = '&';
        private readonly char OperatorAsterisk = '*';
        private readonly char OperatorBackSlash = '\\';
        private readonly char OperatorBang = '!';
        private readonly char OperatorBlockClose = '}';
        private readonly char OperatorBlockOpen = '{';
        private readonly char OperatorBracketsClose = ')';
        private readonly char OperatorBracketsOpen = '(';
        private readonly char OperatorCap = '^';
        private readonly char OperatorColon = ':';
        private readonly char OperatorComma = ',';
        private readonly char OperatorDot = '.';

        private readonly char OperatorDoubleQuote = '"';
        private readonly char OperatorEquality = '=';
        private readonly char OperatorFwdSlash = '/';
        private readonly char OperatorGreaterThan = '>';
        private readonly char OperatorIndexAccessorClose = ']';
        private readonly char OperatorIndexAccessorOpen = '[';
        private readonly string OperatorKey = "operator";
        private readonly char OperatorLessThan = '<';
        private readonly char OperatorMinus = '-';
        private readonly char OperatorNumSign = '#';
        private readonly char OperatorPercent = '%';
        private readonly char OperatorPipe = '|';
        private readonly char OperatorPlus = '+';
        private readonly char OperatorSemicolon = ';';
        private readonly char OperatorSingleQuote = '\'';
        private readonly char OperatorTilde = '~';
        private readonly string OverrideModifier = "override";
        private readonly string PrivateModifier = "private";
        private readonly string ProtectedModifier = "protected";

        private readonly string PublicModifier = "public";
        private readonly string ReturnKey = "return";
        private readonly string StaticModifier = "static";
        private readonly string ConstModifier = "const";
        private readonly string SwitchKey = "switch";
        private readonly string ThisKey = "this";
        private readonly string TryKey = "try";
        private readonly string UsingKey = "using";
        private readonly string VirtualModifier = "virtual";
        private readonly string VoidKey = "void";
        private readonly string WhileKey = "while";

        /// <summary>
        ///     Reserved Key Map
        /// </summary>
        public Dictionary < string, HLTokenType > ReservedKeys =>
            new Dictionary < string, HLTokenType >
            {
                { IfKey, HLTokenType.OpIf },
                { ElseKey, HLTokenType.OpElse },
                { ForEachKey, HLTokenType.OpForEach },
                { ForKey, HLTokenType.OpFor },
                { InKey, HLTokenType.OpIn },
                { IsKey, HLTokenType.OpIs },
                { AsKey, HLTokenType.OpAs },
                { DoKey, HLTokenType.OpDo },
                { WhileKey, HLTokenType.OpWhile },
                { SwitchKey, HLTokenType.OpSwitch },
                { TryKey, HLTokenType.OpTry },
                { CatchKey, HLTokenType.OpCatch },
                { FinallyKey, HLTokenType.OpFinally },
                { UsingKey, HLTokenType.OpUsing },
                { NamespaceKey, HLTokenType.OpNamespace },
                { ClassKey, HLTokenType.OpClass },
                { ContinueKey, HLTokenType.OpContinue },
                { BreakKey, HLTokenType.OpBreak },
                { NewKey, HLTokenType.OpNew },
                { BaseKey, HLTokenType.OpBase },
                { ThisKey, HLTokenType.OpThis },
                { PublicModifier, HLTokenType.OpPublicMod },
                { PrivateModifier, HLTokenType.OpPrivateMod },
                { ProtectedModifier, HLTokenType.OpProtectedMod },
                { VirtualModifier, HLTokenType.OpVirtualMod },
                { AbstractModifier, HLTokenType.OpAbstractMod },
                { OverrideModifier, HLTokenType.OpOverrideMod },
                { StaticModifier, HLTokenType.OpStaticMod },
                { ConstModifier, HLTokenType.OpConstMod },
                { ReturnKey, HLTokenType.OpReturn },
                { VoidKey, HLTokenType.OpTypeVoid },
                { OperatorKey, HLTokenType.OpOperatorImpl }
            };

        /// <summary>
        ///     Valid Member Modifiers.
        /// </summary>
        public Dictionary < string, HLTokenType > MemberModifiers =>
            new Dictionary < string, HLTokenType >
            {
                { ConstModifier, HLTokenType.OpConstMod },
                { PublicModifier, HLTokenType.OpPublicMod },
                { PrivateModifier, HLTokenType.OpPrivateMod }
            };

        /// <summary>
        ///     Valid Class Modifiers
        /// </summary>
        public Dictionary < string, HLTokenType > ClassModifiers =>
            new Dictionary < string, HLTokenType >
            {
                { PublicModifier, HLTokenType.OpPublicMod },
                { PrivateModifier, HLTokenType.OpPrivateMod },
                { ProtectedModifier, HLTokenType.OpProtectedMod },
                { AbstractModifier, HLTokenType.OpAbstractMod },
                { StaticModifier, HLTokenType.OpStaticMod }
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
                { OperatorNumSign, HLTokenType.OpNumSign },
                { OperatorBackSlash, HLTokenType.OpBackSlash },
                { OperatorSingleQuote, HLTokenType.OpSingleQuote },
                { OperatorDoubleQuote, HLTokenType.OpDoubleQuote },
                { OperatorBlockOpen, HLTokenType.OpBlockBracketOpen },
                { OperatorBlockClose, HLTokenType.OpBlockBracketClose },
                { OperatorBracketsOpen, HLTokenType.OpBracketOpen },
                { OperatorBracketsClose, HLTokenType.OpBracketClose },
                { OperatorIndexAccessorOpen, HLTokenType.OpIndexerBracketOpen },
                { OperatorIndexAccessorClose, HLTokenType.OpIndexerBracketClose },
                { OperatorAsterisk, HLTokenType.OpAsterisk },
                { OperatorFwdSlash, HLTokenType.OpFwdSlash },
                { OperatorSemicolon, HLTokenType.OpSemicolon },
                { OperatorComma, HLTokenType.OpComma },
                { OperatorColon, HLTokenType.OpColon },
                { OperatorDot, HLTokenType.OpDot },
                { OperatorPlus, HLTokenType.OpPlus },
                { OperatorMinus, HLTokenType.OpMinus },
                { OperatorPercent, HLTokenType.OpPercent },
                { OperatorEquality, HLTokenType.OpEquality },
                { OperatorAnd, HLTokenType.OpAnd },
                { OperatorPipe, HLTokenType.OpPipe },
                { OperatorCap, HLTokenType.OpCap },
                { OperatorBang, HLTokenType.OpBang },
                { OperatorLessThan, HLTokenType.OpLessThan },
                { OperatorGreaterThan, HLTokenType.OpGreaterThan },
                { OperatorTilde, HLTokenType.OpTilde }
            };

    }

}
