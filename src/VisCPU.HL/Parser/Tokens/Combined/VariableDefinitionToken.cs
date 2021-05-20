using System.Collections.Generic;
using System.Linq;

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
        public IHlToken[] InitializerExpression { get; }

        /// <summary>
        ///     Variable Modifiers
        /// </summary>
        public IHlToken[] Modifiers => m_Modifiers.ToArray();

        private List <IHlToken> m_Modifiers;

        /// <summary>
        ///     Variable Name
        /// </summary>
        public IHlToken Name { get; }

        public IHlToken Size { get; }

        /// <summary>
        ///     Variable FunctionType Name
        /// </summary>
        public IHlToken TypeName { get; }

        #region Public

        public VariableDefinitionToken(
            IHlToken name,
            IHlToken typeName,
            IHlToken[] modifiers,
            IHlToken[] subtokens,
            IHlToken[] initializerExpression ) : this(
                                                      name,
                                                      typeName,
                                                      modifiers,
                                                      subtokens,
                                                      initializerExpression,
                                                      null
                                                     )
        {
        }

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <param name="typeName">Variable FunctionType Name</param>
        /// <param name="modifiers">Variable Modifiers</param>
        /// <param name="subtokens">Child Tokens</param>
        /// <param name="initializerExpression">Initializer Expression</param>
        /// <param name="size">Size of the Variable</param>
        public VariableDefinitionToken(
            IHlToken name,
            IHlToken typeName,
            IHlToken[] modifiers,
            IHlToken[] subtokens,
            IHlToken[] initializerExpression,
            IHlToken size ) : base(
                                   HlTokenType.OpVariableDefinition,
                                   subtokens,
                                   typeName.SourceIndex
                                  )
        {
            m_Modifiers = modifiers.ToList();
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

        public void MakeInternal()
        {
            if (m_Modifiers.All( x => x.Type != HlTokenType.OpInternalMod ) )
                m_Modifiers.Add( new HlTextToken( HlTokenType.OpInternalMod, "internal", -1 ) );
        }

        #endregion

    }

}
