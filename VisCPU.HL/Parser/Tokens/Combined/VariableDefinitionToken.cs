using VisCPU.HL.Parser.Tokens.Expressions;

namespace VisCPU.HL.Parser.Tokens.Combined
{

    /// <summary>
    ///     Implements a Variable(Property) Definition Token
    /// </summary>
    public class VariableDefinitionToken : CombinedToken
    {

        /// <summary>
        ///     Initializer Expression
        /// </summary>
        public readonly HLExpression InitializerExpression;

        /// <summary>
        ///     Variable Modifiers
        /// </summary>
        public readonly IHLToken[] Modifiers;

        /// <summary>
        ///     Variable Name
        /// </summary>
        public readonly IHLToken Name;

        public readonly IHLToken Size;

        /// <summary>
        ///     Variable Type Name
        /// </summary>
        public readonly IHLToken TypeName;

        #region Public

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <param name="typeName">Variable Type Name</param>
        /// <param name="modifiers">Variable Modifiers</param>
        /// <param name="subtokens">Child Tokens</param>
        /// <param name="initializerExpression">Initializer Expression</param>
        public VariableDefinitionToken(
            IHLToken name,
            IHLToken typeName,
            IHLToken[] modifiers,
            IHLToken[] subtokens,
            HLExpression initializerExpression,
            IHLToken size = null ) : base(
                                          HLTokenType.OpVariableDefinition,
                                          subtokens,
                                          typeName.SourceIndex
                                         )
        {
            Modifiers = modifiers;
            Name = name;
            TypeName = typeName;
            InitializerExpression = initializerExpression;
            Size = size;
        }

        /// <summary>
        ///     String Representation of this Token
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Unpack( Modifiers )} {TypeName} {Name}";
        }

        #endregion

    }

}
