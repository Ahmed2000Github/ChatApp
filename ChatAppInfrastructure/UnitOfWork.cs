
using ChatAppCore.Interfaces;
using ChatAppInfrastructure.Repositories;

namespace ChatAppInfrastructure
{

    public class UnitOfWork<T> : IUnitOfWork<T> where T : class
    {
        private readonly DataContext _dataContext;
        private IGenericRepository<T> _entity = null;
        public UnitOfWork(DataContext dataContext)
        {
            this._dataContext = dataContext;
        }


        public IGenericRepository<T> entity
        {
            get
            {
                return _entity ?? (_entity = new GenericRepository<T>(_dataContext));
            }
        }

        public void save()
        {
            _dataContext.SaveChanges();
        }
    }
}
