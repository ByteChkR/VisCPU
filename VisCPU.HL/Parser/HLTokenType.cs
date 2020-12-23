﻿/// <summary>
/// Contains Core Logic and Enums
/// </summary>

namespace VisCPU.HL.Parser
{
    /// <summary>
    ///     Token Type Enum containing all tokens used inside XL
    /// </summary>
    public enum HLTokenType
    {
        Any,
        Unknown,
        OpNone,
        OpNewLine,
        OpPlus,
        OpMinus,
        OpAsterisk,
        OpFwdSlash,
        OpBackSlash,
        OpPercent,
        OpSpace,
        OpNumber,
        OpWord,
        OpBracketOpen,
        OpBracketClose,
        OpBlockBracketOpen,
        OpBlockBracketClose,
        OpIndexerBracketOpen,
        OpIndexerBracketClose,

        OpSemicolon,
        OpComma,
        OpColon,
        OpDot,
        OpEquality,
        OpAnd,
        OpLogicalAnd,
        OpPipe,
        OpLogicalOr,
        OpCap,
        OpBang,
        OpLessThan,
        OpGreaterThan,
        OpLessOrEqual,
        OpGreaterOrEqual,
        OpSingleQuote,
        OpDoubleQuote,
        OpTilde,
        OpComparison,

        //Reserved Key Values
        OpIf,
        OpElse,
        OpForEach,
        OpFor,
        OpIn,
        OpIs,
        OpAs,
        OpDo,
        OpWhile,
        OpSwitch,
        OpTry,
        OpCatch,
        OpFinally,
        OpUsing,
        OpNamespace,
        OpClass,
        OpContinue,
        OpBreak,
        OpNew,
        OpBase,
        OpThis,
        OpPublicMod,
        OpPrivateMod,
        OpProtectedMod,
        OpVirtualMod,
        OpAbstractMod,
        OpOverrideMod,
        OpStaticMod,

        OpComment,

        OpNamespaceDefinition,
        OpClassDefinition,
        OpVariableDefinition,
        OpFunctionDefinition,
        OpExpression,
        OpStringLiteral,
        OpStatement,
        OpTypeVoid,
        OpBlockToken,
        OpReturn,
        OpInvocation,
        OpArrayAccess,
        OpRuntimeNamespace,
        OpOperatorImpl,
        OpPropertyDefinition,

        OpUnaryIncrement,
        OpUnaryDecrement,
        OpSumAssign,
        OpDifAssign,
        OpProdAssign,
        OpQuotAssign,
        OpRemAssign,
        OpOrAssign,
        OpXOrAssign,
        OpAndAssign,

        OpNumSign,

        //End Of File
        EOF
    }
}