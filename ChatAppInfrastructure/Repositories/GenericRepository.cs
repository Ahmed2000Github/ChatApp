using ChatAppCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ChatAppInfrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DataContext _dataContext;
        private DbSet<T> table = null;

        public GenericRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
            table = _dataContext.Set<T>();
        }

        public void Delete(object id)
        {
            T element = GetById(id);
            table.Remove(element);
        }
        public void DeleteRange(params T[] elements)
        {
            table.RemoveRange(elements);
        }
        public void UpdateRange(params T[] elements)
        {
            table.UpdateRange(elements);
        }

        public IEnumerable<T> GetAll()
        {
            return table.ToList<T>();
        }

        public T GetById(object id)
        {
            return table.Find(id);
        }
        public IEnumerable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return table.Where(predicate);
        }

        public IEnumerable<T> GetFull<TProperty>( Expression<Func<T, TProperty>> navigationPropertyPath)
        {
            return table.Include(navigationPropertyPath);
        }
        public void Update(T entity)
        {
            table.Attach(entity);
            _dataContext.Entry(entity).State = EntityState.Modified;
        }

        void IGenericRepository<T>.Add(T entity)
        {
            table.Add(entity);
        }


    }
}
