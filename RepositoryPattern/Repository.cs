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

        public virtual void InsertOrUpdate(TEntity entity)
        {
            DbSet.AddOrUpdate(entity);    // using System.Data.Entity.Migrations;
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

        public virtual IQueryable<TEntity> Table => DbSet;

        public virtual IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate)
        {
            IQueryable<TEntity> query = DbSet;

            if (includeProperties != null && includeProperties.Length > 0)
            {
                foreach (Expression<Func<TEntity, object>> include in includeProperties)
                    query = query.Include(include);
            }

            return query.Where(predicate);
        }     

        public void Refresh(TEntity entity)
        {
            if (entity == null) return;

            ((IObjectContextAdapter)Context).ObjectContext.RefreshAsync(RefreshMode.StoreWins, entity);
        }   

        public void Refresh(IEnumerable<TEntity> entities)
        {
            if (entities == null || entities.Count == 0) return;

            ((IObjectContextAdapter)Context).ObjectContext.RefreshAsync(RefreshMode.StoreWins, entities);
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