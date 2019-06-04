public interface IRepository<TEntity> : IDisposable
        where TEntity : class
    {
        void Insert(TEntity entity);
        void Update(TEntity entity);
        void Delete(object id);
        void Delete(TEntity entity);
        void DeleteAll(Expression<Func<TEntity, bool>> predicate);
        Task Save();
        Task<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> FindById(object id);
        IQueryable<TEntity> Table { get; }
        IQueryable<TEntity> FindAll();
        IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate);
        IQueryable<TEntity> FindAllIncluding(params Expression<Func<TEntity, object>>[] includeProperties);
    }