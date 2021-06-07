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
        /// Creates and returns an implication expression P ⇒ Q.
        /// As per the definition of implication, <code>Implies(model => model.P, model => model.Q)</code> is exactly equivalent to (though hopefully easier to read than)
        /// <code>
        /// model => !model.P || model.Q
        /// </code>
        /// </summary>
        /// <typeparam name="TDomain">The model type.</typeparam>
        /// <param name="p">The antecedent expression.</param>
        /// <param name="q">The consequent expression.</param>
        /// <returns>An implication expression.</returns>
        public static Expression<Predicate<TDomain>> Implies(Expression<Predicate<TDomain>> p, Expression<Predicate<TDomain>> q)
        {
            // We essentially want !p.Body || q.Body BUT with the parameter expressions in each replaced with a new singular one.
            // That's what ParameterReplacer does for us.
            var pr = new ParameterReplacer(p.Parameters[0].Name);

            return Expression.Lambda<Predicate<TDomain>>(
                Expression.OrElse(
                    Expression.IsFalse(pr.VisitLambdaBody(p)),
                    pr.VisitLambdaBody(q)),
                pr.NewParameter);
        }

        /// <summary>
        /// Creates and returns an equivalence expression. That is, P ⇔ Q.
        /// </summary>
        /// <typeparam name="TDomain"></typeparam>
        /// <param name="p"></param>
        /// <param name="q"></param>
        /// <returns>An equivalence expression.</returns>
        public static Expression<Predicate<TDomain>> Iff(Expression<Predicate<TDomain>> p, Expression<Predicate<TDomain>> q)
        {
            // Note that we do this as (P ⇒ Q) ∧ (Q ⇒ P) rather than anything shorter (like P == Q) because it means that the expression is already
            // in conjunctive normal form - to make it easier to apply resolution. See Artifical Intelligence: A Modern Approach or an equivalent
            // learning resource for details.
            // There's also probably a more direct way to write this instead doing parameter replacement three times - but going for readability rather
            // than efficiency for the moment..
            var pr = new ParameterReplacer(p.Parameters[0].Name);

            var clause1 = Implies(p, q);
            var clause2 = Implies(q, p);
            return Expression.Lambda<Predicate<TDomain>>(Expression.AndAlso(pr.VisitLambdaBody(clause1), pr.VisitLambdaBody(clause2)), pr.NewParameter);
        }

        /// <summary>
        /// Expression visitor that replaces all parameter references (of the TDomain type) with a singular one of its own.
        /// </summary>
        private class ParameterReplacer : ExpressionVisitor
        {
            public ParameterReplacer(string name) => NewParameter = Expression.Parameter(typeof(TDomain), name);

            public ParameterExpression NewParameter { get; private set; }

            public override Expression Visit(Expression node)
            {
                if (node is ParameterExpression parameterExpression && parameterExpression.Type == NewParameter.Type)
                {
                    return NewParameter;
                }

                return base.Visit(node);
            }

            public Expression VisitLambdaBody(Expression<Predicate<TDomain>> lambda) => Visit(lambda.Body);
        }
    }
}
