using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ApplicationCore.Interfaces
{
    public interface IRepository<T>
    {
        T GetById(Guid id);
        IEnumerable<T> List();
        IEnumerable<T> List(Expression<Func<T, bool>> expression);
        IEnumerable<T> List(ISpecification<T> spec);
        void Add(T entity);
        void Delete(T entity);
        void Update(T entity);
    }
}