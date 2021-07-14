using System;
using System.Linq.Expressions;

namespace LinqToKB.PropositionalLogic.InternalUtilities
{
    /// <summary>
    /// Expression visitor that replaces all parameter references (of the <see cref="TModel" /> type) with a singular one of its own.
    /// </summary>
    internal class ParameterReplacer<TModel> : ExpressionVisitor
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
