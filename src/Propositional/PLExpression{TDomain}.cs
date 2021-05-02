using System;
using System.Linq.Expressions;

namespace LinqToKB.Propositional
{
    /// <summary>
    /// Factory methods for propositional logic expressions.
    /// </summary>
    /// <typeparam name="TDomain">The domain of discourse.</typeparam>
    public static class PLExpression<TDomain>
    {
        /// <summary>
        /// Creates and returns an implication expression P ⇒ Q (or equivalently, ¬P ∨ Q). As such, exactly equivalent to (though hopefully easier to read than) a lambda similar to:
        /// <code>
        /// m => !m.P || m.Q
        /// </code>
        /// </summary>
        /// <typeparam name="TDomain">The model type.</typeparam>
        /// <param name="p">The antecedent expression.</param>
        /// <param name="q">The consequent expression.</param>
        /// <returns>An implication expression.</returns>
        public static Expression<Predicate<TDomain>> Implies(Expression<Predicate<TDomain>> p, Expression<Predicate<TDomain>> q)
        {
            // We essentially want to !p.Bpdy || q.Body BUT with the parameter expressions in each replaced with a new singular one..
            var newParameter = Expression.Parameter(typeof(TDomain));
            var parameterReplacer = new ParameterReplacerVisitor(newParameter);
            return Expression.Lambda<Predicate<TDomain>>(Expression.OrElse(Expression.IsFalse(parameterReplacer.Visit(p.Body)), parameterReplacer.Visit(q.Body)), newParameter);
        }

        /// <summary>
        /// Creates and returns an equivalence expression. That is, P ⇔ Q. Essentially just a shorthand for antecedent == consequent.
        /// </summary>
        /// <typeparam name="TDomain"></typeparam>
        /// <param name="p"></param>
        /// <param name="q"></param>
        /// <returns>An equivalence expression.</returns>
        public static Expression<Predicate<TDomain>> Iff(Expression<Predicate<TDomain>> p, Expression<Predicate<TDomain>> q)
        {
            // We essentially want to p.Bpdy == q.Body BUT with the parameter expressions in each replaced with a new singular one..
            var newParameter = Expression.Parameter(typeof(TDomain));
            var parameterReplacer = new ParameterReplacerVisitor(newParameter);
            return Expression.Lambda<Predicate<TDomain>>(Expression.Equal(parameterReplacer.Visit(p.Body), parameterReplacer.Visit(q.Body)), newParameter);
        }

        private class ParameterReplacerVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression newParameter;

            public ParameterReplacerVisitor(ParameterExpression newParameter)
            {
                this.newParameter = newParameter;
            }

            public override Expression Visit(Expression node)
            {
                if (node is ParameterExpression parameterExpression && parameterExpression.Type == newParameter.Type)
                {
                    return newParameter;
                }

                return base.Visit(node);
            }
        }
    }
}
