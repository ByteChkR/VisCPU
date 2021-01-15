﻿namespace VisCPU.HL.Parser.Tokens.Combined
{

    /// <summary>
    ///     Implements a Variable(Property) Definition Token
    /// </summary>
    public class VariableDefinitionToken : CombinedToken
    {

        /// <summary>
        ///     Initializer Expression
        /// </summary>
        public readonly IHlToken[] InitializerExpression;

        /// <summary>
        ///     Variable Modifiers
        /// </summary>
        public readonly IHlToken[] Modifiers;

        /// <summary>
        ///     Variable Name
        /// </summary>
        public readonly IHlToken Name;

        public readonly IHlToken Size;

        /// <summary>
        ///     Variable Type Name
        /// </summary>
        public readonly IHlToken TypeName;

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
            IHlToken name,
            IHlToken typeName,
            IHlToken[] modifiers,
            IHlToken[] subtokens,
            IHlToken[] initializerExpression,
            IHlToken size = null ) : base(
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
