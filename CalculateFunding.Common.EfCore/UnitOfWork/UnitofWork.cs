using CalculateFunding.Common.EfCore.GenericRepository;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculateFunding.Common.EfCore.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private DbContext _context = null;
        private Dictionary<string, object> repositories;
        private bool disposed = false;

        public UnitOfWork(DbContext dbContext)
        {
            this._context = dbContext;
        }

        public DbContext context
        {
            get
            {
                return this._context;
            }
            set
            {
                this._context = value;
            }
        }
        public void Commit()
        {
            using (var transaction = this._context.Database.BeginTransaction())
            {
                try
                {

                    this.context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    Rollback(transaction);
                    if (e.InnerException != null) throw e.InnerException;
                }             
            }
        }
        public async Task CommitAsync()
        {
            using (var transaction = this._context.Database.BeginTransaction())
            {
                try
                {
                    this.context.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    Rollback(transaction);
                    throw e;
                }
            }
        }
        private void Rollback(IDbContextTransaction transaction)
        {
            transaction.Rollback();

            foreach (var entry in this._context.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Unchanged;
                        break;
                }
            }
        }

        public GenericRepository<T> GenericRepository<T>() where T : class
        {
            if (repositories == null)
            {
                repositories = new Dictionary<string, object>();
            }

            var type = typeof(T).Name;

            if (!repositories.ContainsKey(type))
            {
                var repType = typeof(GenericRepository<>);
                var repInstance = Activator.CreateInstance(repType.MakeGenericType(typeof(T)), context);
                repositories.Add(type, repInstance);
            }

            return (GenericRepository<T>)repositories[type];

        }

        #region Implementing IDiosposable...    
        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {                
                    _context.Dispose();
                }
            }
            this.disposed = true;

        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
