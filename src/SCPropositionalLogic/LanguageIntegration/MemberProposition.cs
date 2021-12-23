using System.Reflection;

namespace SCPropositionalLogic.LanguageIntegration
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
        /// <remarks>
        /// Internal because for the instance to make sense the memberInfo has to adhere to some constraints - 
        /// and the logic validating that isn't in this class, its in SentenceFactory.
        /// </remarks>
        internal MemberProposition(MemberInfo memberInfo)
            : base(new MemberSymbol(memberInfo))
        {
        }

        /// <summary>
        /// Representation of a <see cref="Proposition"/> symbol that refers to a particular class member.
        /// </summary>
        internal class MemberSymbol
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MemberSymbol"/> class.
            /// </summary>
            /// <param name="memberInfo"></param>
            public MemberSymbol(MemberInfo memberInfo) => MemberInfo = memberInfo;

            /// <summary>
            /// Gets the <see cref="MemberInfo"/> to which this symbol refers.
            /// </summary>
            public MemberInfo MemberInfo { get; }

            /// <inheritdoc />
            public override string ToString() => MemberInfo.Name;

            /// <inheritdoc />
            public override bool Equals(object obj)
            {
                return obj is MemberSymbol otherMemberSymbol
                    && MemberInfoEqualityComparer.Instance.Equals(MemberInfo, otherMemberSymbol.MemberInfo);
            }

            /// <inheritdoc />
            public override int GetHashCode()
            {
                return MemberInfoEqualityComparer.Instance.GetHashCode(this.MemberInfo);
            }
        }
    }
}
