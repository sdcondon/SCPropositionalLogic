using System;
using System.Linq.Expressions;

namespace LinqToKnowledgeBase.Propositional
{
    /// <summary>
    /// Factory methods for propositional logic expressions.
    /// </summary>
    /// <typeparam name="TModel">The type of model dealt with by this class.</typeparam>
    public static class PLExpression<TModel>
    {
        /// <summary>
        /// Creates and returns an implication expression P ⇒ Q.
        /// As per the definition of implication, <code>Implies(model => model.P, model => model.Q)</code> is exactly equivalent to (though hopefully easier to read than)
        /// <code>
        /// model => !model.P || model.Q
        /// </code>
        /// </summary>
        /// <param name="p">The antecedent expression.</param>
        /// <param name="q">The consequent expression.</param>
        /// <returns>An implication expression.</returns>
        public static Expression<Predicate<TModel>> Implies(Expression<Predicate<TModel>> p, Expression<Predicate<TModel>> q)
        {
            // We essentially want !p.Body || q.Body, but with the parameter expressions in each replaced with a new singular one.
            // That's what ParameterReplacer does for us. Note that the name of the parameter from the first is used. Should we
            // complain if the name of the parameter in the second is different? Seems overkill..
            var pr = new ParameterReplacer(p.Parameters[0].Name);

            return Expression.Lambda<Predicate<TModel>>(
                Expression.OrElse(
                    Expression.IsFalse(pr.VisitLambdaBody(p)),
                    pr.VisitLambdaBody(q)),
                pr.NewParameter);
        }

        /// <summary>
        /// Creates and returns an equivalence expression. That is, P ⇔ Q.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="q"></param>
        /// <returns>An equivalence expression.</returns>
        public static Expression<Predicate<TModel>> Iff(Expression<Predicate<TModel>> p, Expression<Predicate<TModel>> q)
        {
            // Note that we do this as (P ⇒ Q) ∧ (Q ⇒ P) rather than anything shorter (like P == Q) because it means that the expression is already
            // in conjunctive normal form - to make it easier to apply resolution. See Artifical Intelligence: A Modern Approach or an equivalent
            // learning resource for details.
            // There's also probably a more direct way to write this instead doing parameter replacement three times - but going for readability rather
            // than efficiency for the moment..
            var pr = new ParameterReplacer(p.Parameters[0].Name);

            var clause1 = Implies(p, q);
            var clause2 = Implies(q, p);
            return Expression.Lambda<Predicate<TModel>>(Expression.AndAlso(pr.VisitLambdaBody(clause1), pr.VisitLambdaBody(clause2)), pr.NewParameter);
        }

        /// <summary>
        /// Expression visitor that replaces all parameter references (of the <see cref="TModel" /> type) with a singular one of its own.
        /// </summary>
        private class ParameterReplacer : ExpressionVisitor
        {
            public ParameterReplacer(string name) => NewParameter = Expression.Parameter(typeof(TModel), name);

            public ParameterExpression NewParameter { get; private set; }

            public override Expression Visit(Expression node)
            {
                if (node is ParameterExpression parameterExpression && parameterExpression.Type == NewParameter.Type)
                {
                    return NewParameter;
                }

                return base.Visit(node);
            }

            public Expression VisitLambdaBody(Expression<Predicate<TModel>> lambda) => Visit(lambda.Body);
        }
    }
}
