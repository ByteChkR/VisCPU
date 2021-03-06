﻿/// <summary>
/// Contains Core Logic and Enums
/// </summary>

namespace VisCPU.HL.Parser
{

    /// <summary>
    ///     Token FunctionType Enum containing all tokens used inside XL
    /// </summary>
    public enum HlTokenType
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
        OpDecimalNumber,
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
        OpNamespaceSeparator,
        OpClass,
        OpContinue,
        OpBreak,
        OpNew,
        OpDelete,
        OpBase,
        OpThis,
        OpPublicMod,
        OpPrivateMod,
        OpInternalMod,
        OpProtectedMod,
        OpVirtualMod,
        OpAbstractMod,
        OpOverrideMod,
        OpStaticMod,
        OpConstMod,
        OpPackedMod,

        OpComment,

        OpNamespaceDefinition,
        OpClassDefinition,
        OpVariableDefinition,
        OpFunctionDefinition,
        OpExpression,
        OpStringLiteral,
        OpCharLiteral,
        OpStatement,
        OpTypeVoid,
        OpBlockToken,
        OpReturn,
        OpInvocation,
        OpArrayAccess,
        OpRuntimeNamespace,
        OpOperatorImpl,
        OpPropertyDefinition,

        OpUnaryPostfixIncrement,
        OpUnaryPostfixDecrement,
        OpUnaryPrefixIncrement,
        OpUnaryPrefixDecrement,
        OpDeReference,
        OpReference,
        OpSumAssign,
        OpDifAssign,
        OpProdAssign,
        OpQuotAssign,
        OpRemAssign,
        OpOrAssign,
        OpXOrAssign,
        OpAndAssign,
        OpShiftLeft,
        OpShiftRight,
        OpShiftLeftAssign,
        OpShiftRightAssign,

        OpNumSign,

        //End Of File
        Eof

    }

}
