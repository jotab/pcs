using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ApplicationCore.Interfaces;

namespace ApplicationCore.Specifications
{
    public class BaseSpecification<T> : ISpecification<T>
    {
        public BaseSpecification()
        {
        }

        public BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria.Add(criteria);
        }

        public List<Expression<Func<T, bool>>> Criteria { get; } = new List<Expression<Func<T, bool>>>();
        public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();
        public List<string> IncludeStrings { get; } = new List<string>();

        public ISpecification<T> Where(Expression<Func<T, bool>> expression)
        {
            Criteria.Add(expression);
            return this;
        }

        protected virtual void Include(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        // string-based includes allow for including children of children, e.g. Basket.Items.Product
        protected virtual void Include(string includeString)
        {
            IncludeStrings.Add(includeString);
        }
    }
}