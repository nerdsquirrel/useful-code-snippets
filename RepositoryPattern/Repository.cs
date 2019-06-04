public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        internal  ApplicationDbContext Context;
        internal  DbSet<TEntity> DbSet;

        public Repository(ApplicationDbContext context)
        {
            Context = context;
            DbSet = Context.Set<TEntity>();
        }

        public virtual void Insert(TEntity entity)
        {
            DbSet.Add(entity);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            DbSet.Attach(entityToUpdate);
            Context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public virtual void Delete(object id)
        {
            var entityToDelete = DbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (Context.Entry(entityToDelete).State == EntityState.Detached)
            {
                DbSet.Attach(entityToDelete);
            }
            DbSet.Remove(entityToDelete);
        }

        public virtual void DeleteAll(Expression<Func<TEntity, bool>> predicate)
        {
            var entityListToDelete = DbSet.Where(predicate);
            foreach (var entityToDelete in entityListToDelete)
            {
                Delete(entityToDelete);
            }
        }

        public virtual async Task Save()
        {
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    await Context.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
            }
        }

        public virtual async Task<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbSet.Where(predicate).FirstOrDefaultAsync();
        }

        public virtual async Task<TEntity> FindById(object id)
        {
            return await DbSet.FindAsync(id);
        }

        public virtual IQueryable<TEntity> FindAll()
        {
            return DbSet;
        }

        public virtual IQueryable<TEntity> Table => DbSet;

        public virtual IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.Where(predicate);
        }

        public virtual IQueryable<TEntity> FindAllIncluding(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return includeProperties.Aggregate<Expression<Func<TEntity, object>>, IQueryable<TEntity>>(DbSet,
                (current, property) => current.Include(property));
        }

        #region IDisposable
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Context.Dispose();
                Context = null;
            }
        }

        #endregion
    }