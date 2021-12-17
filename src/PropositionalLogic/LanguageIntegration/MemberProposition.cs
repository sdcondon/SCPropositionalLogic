using System.Reflection;

namespace LinqToKB.PropositionalLogic.LanguageIntegration
{
    /// <summary>
    /// Representation of a proposition sentence of first order logic.
    /// Specifically, represents a proposition that refers to a boolean property of a class representing the model.
    /// </summary>
    /// <remarks>
    /// TODO-FUNCTIONALITY: Might ultimately be useful to make the Member.. classes generic in the same way as KnowledgeBase - for
    /// validation, as well as potential manipulation power. OR simply delete this class as it adds no real value.
    /// </remarks>
    public class MemberProposition : Proposition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemberProposition"/> class.
        /// </summary>
        /// <param name="memberInfo"></param>
        public MemberProposition(MemberInfo memberInfo)
            : base(new MemberSymbol(memberInfo))
        {
        }
    }
}
