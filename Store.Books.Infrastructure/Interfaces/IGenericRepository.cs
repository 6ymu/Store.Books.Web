using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Store.Books.Infrastructure.Interfaces
{
    public interface IGenericRepository<T>
    {
        Task<IEnumerable<T>> Get(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "");

        Task<T> GetById(object id);

        Task Insert(T entity);

        Task Delete(object id);

        Task Delete(T entityToDelete);

        Task Update(T entityToUpdate);

        IEnumerable<T> GetPaged(int page = 1, int perPage = 20,
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "");


    }
}
