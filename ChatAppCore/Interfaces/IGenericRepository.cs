
using System.Linq.Expressions;

namespace ChatAppCore.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        public T GetById(object id);
        public IEnumerable<T> GetAll();
        public IEnumerable<T> GetFull<TProperty>(Expression<Func<T, TProperty>> navigationPropertyPath);
        public IEnumerable<T> Where(Expression<Func<T, bool>> predicate);
        public void Add(T entity);
        public void Update(T entity);
        public void Delete(object id);
        public void DeleteRange(params T[] elements);
        public void UpdateRange(params T[] elements);
    }
}
