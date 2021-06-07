using System.Linq.Expressions;

namespace LinqToKB.Propositional.Internals
{
    /// <summary>
    /// Linq expression visitor that converts the visited expression to conjunctive normal form.
    /// </summary>
    public class CNFConverter : ExpressionVisitor
    {
        private readonly NegationNormalFormConverter negationNormalFormConverter = new NegationNormalFormConverter();
        private readonly OrDistributor orDistributor = new OrDistributor();

        /// <inheritdoc />
        public override Expression Visit(Expression node)
        {
            // Need to completely convert to NNF before distributing ORs,
            // else stuff will be missed. Hence the separate subconverters.
            node = negationNormalFormConverter.Visit(node);
            node = orDistributor.Visit(node);
            return node;
        }

        /// <summary>
        /// Expression visitor that converts to negation normal form by repeated elimination of double negatives and application of de Morgans laws.
        /// </summary>
        private class NegationNormalFormConverter : ExpressionVisitor
        {
            /// <inheritdoc />
            public override Expression Visit(Expression node)
            {
                // TODO: throw for anything that can't be used? whitelist is AndAlso, OrElse, IsFalse, Parameter access (of a bool - but that implied by other limitations)?
                // Or (better): just stop and return node as soon as we hit anything with non-boolean-valued children (i.e. treat it as an atomic sentence)?

                if (node is UnaryExpression u && u.NodeType == ExpressionType.IsFalse)
                {
                    if (u.Operand is UnaryExpression isFalse && isFalse.NodeType == ExpressionType.IsFalse)
                    {
                        // Eliminate double negative: ¬(¬P) ≡ P
                        node = isFalse.Operand;
                    }
                    else if (u.Operand is BinaryExpression andAlso && andAlso.NodeType == ExpressionType.AndAlso)
                    {
                        // Apply de Morgan: ¬(P ∧ Q) ≡ (¬P ∨ ¬Q)
                        node = Expression.OrElse(Expression.IsFalse(andAlso.Left), Expression.IsFalse(andAlso.Right));
                    }
                    else if (u.Operand is BinaryExpression orElse && orElse.NodeType == ExpressionType.OrElse)
                    {
                        // Apply de Morgan: ¬(P ∨ Q) ≡ (¬P ∧ ¬Q)
                        node = Expression.AndAlso(Expression.IsFalse(orElse.Left), Expression.IsFalse(orElse.Right));
                    }
                }

                return base.Visit(node);
            }
        }

        /// <summary>
        /// Expression visitor that recursively distributes disjunctions over conjunctions.
        /// </summary>
        private class OrDistributor : ExpressionVisitor
        {
            /// <inheritdoc />
            public override Expression Visit(Expression node)
            {
                if (node is BinaryExpression b && b.NodeType == ExpressionType.OrElse)
                {
                    if (b.Right is BinaryExpression andAlsoRight && andAlsoRight.NodeType == ExpressionType.AndAlso)
                    {
                        // Apply distribution of ∨ over ∧: (α ∨ (β ∧ γ)) ≡ ((α ∧ β) ∨ (α ∧ γ))
                        node = Expression.AndAlso(
                            Expression.OrElse(b.Left, andAlsoRight.Left),
                            Expression.OrElse(b.Left, andAlsoRight.Right));
                    }
                    else if (b.Left is BinaryExpression andAlsoLeft && andAlsoLeft.NodeType == ExpressionType.AndAlso) // hmm. else. suspect can find some failing test cases, here..
                    {
                        // Apply distribution of ∨ over ∧: (α ∨ (β ∧ γ)) ≡ ((α ∧ β) ∨ (α ∧ γ))
                        node = Expression.AndAlso(
                            Expression.OrElse(andAlsoLeft.Left, b.Right),
                            Expression.OrElse(andAlsoLeft.Right, b.Right));
                    }
                }

                return base.Visit(node);
            }
        }
    }
}
