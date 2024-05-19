

namespace ChatAppCore.Interfaces
{
    public interface IUnitOfWork<T> where T : class
    {
        IGenericRepository<T> entity { get; }
        public void save();
    }

}
