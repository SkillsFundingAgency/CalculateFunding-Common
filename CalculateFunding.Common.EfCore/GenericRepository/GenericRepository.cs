using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CalculateFunding.Common.EfCore.GenericRepository
{
    public class GenericRepository<T> where T : class
    {
        private readonly DbContext context;
        private DbSet<T> entities;

        public GenericRepository(DbContext context)
        {
            this.context = context;
        }

        public DbSet<T> Entities
        {
            get
            {
                if (entities == null)
                {
                    entities = context.Set<T>();
                }

                return entities;

            }
        }

        public virtual IQueryable<T> Entity
        {
            get
            {
                return this.Entities.AsQueryable();
            }
        }

        public virtual bool Exists(object primaryKey)
        {
            return Entities.Find(primaryKey) != null;
        }
        public virtual IEnumerable<T> Get()
        {
            IQueryable<T> lstObj = Entities;
            return lstObj.AsNoTracking().ToList();
        }
        public virtual async Task<IEnumerable<T>> GetAllAsyn()
        {
            return await context.Set<T>().ToListAsync();
        }
        public virtual T GetById(int id)
        {
            return Entities.Find(id);
        }

        public virtual async Task<T> GetAsync(int id)
        {
            return await context.Set<T>().FindAsync(id);
        }
        /// <summary>
        /// generic method to get many record on the basis of a condition.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> GetMany(Func<T, bool> where)
        {
            return Entities.AsNoTracking().Where(where).ToList();
        }
        /// <summary>
        /// generic method to get many record on the basis of a condition but query able.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual IQueryable<T> GetManyAsQueryable(Expression<Func<T, Boolean>> where)
        {
            return Entity.AsNoTracking().Where(where);
        }
        /// <summary>
        /// generic get method , fetches data for the entities on the basis of condition.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual T GetFirstorDefault(Func<T, Boolean> where)
        {
            return Entities.AsNoTracking().Where(where).FirstOrDefault();
        }


        /// <summary>
        /// Gets a single record by the specified criteria (usually the unique identifier)
        /// </summary>
        /// <param name="predicate">Criteria to match on</param>
        /// <returns>A single record that matches the specified criteria</returns>
        public T GetSingle(Func<T, bool> predicate)
        {
            return Entities.AsNoTracking().Single<T>(predicate);
        }

        public T GetSingleOrDefault(Func<T, bool> predicate)
        {
            return Entities.AsNoTracking().SingleOrDefault<T>(predicate);
        }

        public T GetSingleAsQueryable(Expression<Func<T, bool>> predicate)
        {
            return Entity.AsNoTracking().Single<T>(predicate);
        }

        public T GetFirstAsQueryable(Expression<Func<T, bool>> predicate)
        {
            return Entity.AsNoTracking().FirstOrDefault<T>(predicate);
        }

        /// <summary>
        /// Fetch the First or default item asynchronously
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate) => await Entity.AsNoTracking().FirstOrDefaultAsync(predicate);
       
        /// <summary>
        /// Fetch the single or default item asynchronously
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate) => await Entity.AsNoTracking().SingleOrDefaultAsync(predicate);

        /// <summary>
        /// Get the Max of a selector asynchronously
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        public async Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> selector) => await Entity.AsNoTracking().MaxAsync(selector);


        /// <summary>
        /// The first record matching the specified criteria
        /// </summary>
        /// <param name="predicate">Criteria to match on</param>
        /// <returns>A single record containing the first record matching the specified criteria</returns>
        public T GetFirst(Func<T, bool> predicate)
        {
            return Entities.AsNoTracking().First<T>(predicate);
        }

        /// <summary>
        /// Inclue multiple
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        public IQueryable<T> GetWithInclude(Expression<Func<T, bool>> predicate, params string[] include)
        {
            IQueryable<T> query = this.Entities;
            query = include.Aggregate(query, (current, inc) => current.Include(inc));
            return query.Where(predicate);
        }

        /// <summary>
        /// Query the database with joins and selects
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="filter"></param>
        /// <param name="select"></param>
        /// <param name="join"></param>
        /// <returns></returns>
        public IQueryable<TResult> Query<TResult>(
            Expression<Func<T, bool>> filter,
            Expression<Func<T, TResult>> select,
            Func<IQueryable<T>, IQueryable<TResult>> join = null)
        {
            var query = Entities.Where(filter);

            return (join != null) ? join(query) : query.Select(select);
        }

        public virtual void Insert(T entity)
        {
            Entities.Add(entity);
        }

        public virtual void Insert(IEnumerable<T> EntityList)
        {
            foreach (T entity in EntityList)
            {
                Entities.Add(entity);
            }  
        }

        public virtual async void BulkInsertAsync(IEnumerable<T> EntityList)
        {
            await Entities.AddRangeAsync(EntityList);
        }

        public virtual void Delete(object id)
        {
            T entity = Entities.Find(id);
            Delete(entity);
        }
        public virtual void Delete(int id)
        {
            T entity = Entities.Find(id);
            Delete(entity);
        }
        public virtual void Delete(long id)
        {
            T entity = Entities.Find(id);
            Delete(entity);
        }
        public virtual void Delete(T entity)
        {
            if (context.Entry(entity).State == EntityState.Detached)
            {
                Entities.Attach(entity);
            }
            Entities.Remove(entity);
        }

        public void Delete(Expression<Func<T, Boolean>> where)
        {
            IQueryable<T> objList = Entities.Where<T>(where).AsQueryable();
            foreach (T obj in objList)
            {
                Entities.Remove(obj);
            }
        }
        public virtual void Update(T entity)
        {
            Entities.Attach(entity);
            context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Update(IEnumerable<T> EntityList)
        {
            try
            {
                foreach (T entity in EntityList)
                {
                    Entities.Attach(entity);
                    context.Entry(entity).State = EntityState.Modified;
                }

            }
            catch (Exception e)
            {
                throw;
            }

        }

        public virtual void BulkUpdate(IEnumerable<T> EntityList)
        {
            try
            {
                Entities.AttachRange(EntityList);
                foreach (T entity in EntityList)
                {
                    context.Entry(entity).State = EntityState.Modified;
                }
            }
            catch (Exception e)
            {
                throw;
            }

        }

    }
}
