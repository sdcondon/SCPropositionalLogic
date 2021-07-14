using System.Linq.Expressions;

namespace LinqToKB.PropositionalLogic
{
    /// <summary>
    /// Linq expression visitor that converts the visited expression to conjunctive normal form.
    /// </summary>
    public class CNFConverter : ExpressionVisitor
    {
        private readonly NegationNormalFormConverter negationNormalFormConverter = new NegationNormalFormConverter();
        private readonly OrDistributor orDistributor = new OrDistributor();

        /// <summary>
        /// Converts an expression to conjunctive normal form.
        /// </summary>
        /// <typeparam name="T">The expression type.</typeparam>
        /// <param name="expression">The expression to convert.</param>
        /// <returns>The expression, converted to conjunctive normal form.</returns>
        public static T ConvertToCNF<T>(T expression)
            where T : Expression
        {
            return new CNFConverter().VisitAndConvert(expression, nameof(CNFConverter));
        }

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
                // TODO-ROBUSTNESS: throw for anything that can't be used? whitelist is AndAlso, OrElse, Not, Parameter access (of a bool - but that implied by other limitations)?
                // Or (better): just stop and return node as soon as we hit anything with non-boolean-valued children (i.e. treat it as an atomic sentence)?
                // TODO-MAINTAINABILITY: this feels like a more fundamental bit of logic than specific to CNF? Considering a redesign where PLExpression<> is instantiable..

                if (node is UnaryExpression u && u.NodeType == ExpressionType.Not)
                {
                    if (u.Operand is UnaryExpression not && not.NodeType == ExpressionType.Not)
                    {
                        // Eliminate double negative: ¬(¬P) ≡ P
                        node = not.Operand;
                    }
                    else if (u.Operand is BinaryExpression andAlso && andAlso.NodeType == ExpressionType.AndAlso)
                    {
                        // Apply de Morgan: ¬(P ∧ Q) ≡ (¬P ∨ ¬Q)
                        node = Expression.OrElse(Expression.Not(andAlso.Left), Expression.Not(andAlso.Right));
                    }
                    else if (u.Operand is BinaryExpression orElse && orElse.NodeType == ExpressionType.OrElse)
                    {
                        // Apply de Morgan: ¬(P ∨ Q) ≡ (¬P ∧ ¬Q)
                        node = Expression.AndAlso(Expression.Not(orElse.Left), Expression.Not(orElse.Right));
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
                        // Apply distribution of ∨ over ∧: (α ∨ (β ∧ γ)) ≡ ((α ∨ β) ∧ (α ∨ γ))
                        node = Expression.AndAlso(
                            Expression.OrElse(b.Left, andAlsoRight.Left),
                            Expression.OrElse(b.Left, andAlsoRight.Right));
                    }
                    else if (b.Left is BinaryExpression andAlsoLeft && andAlsoLeft.NodeType == ExpressionType.AndAlso) // TODO: hmm. else. need to revisit this to verify no bugs..
                    {
                        // Apply distribution of ∨ over ∧: ((β ∧ γ) ∨ α) ≡ ((β ∨ α) ∧ (γ ∨ α))
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
