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
            return lstObj.ToList();
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
            return Entities.Where(where).ToList();
        }
        /// <summary>
        /// generic method to get many record on the basis of a condition but query able.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual IQueryable<T> GetManyAsQueryable(Expression<Func<T, Boolean>> where)
        {
            return Entity.Where(where);
        }
        /// <summary>
        /// generic get method , fetches data for the entities on the basis of condition.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual T GetFirstorDefault(Func<T, Boolean> where)
        {
            return Entities.Where(where).FirstOrDefault();
        }


        /// <summary>
        /// Gets a single record by the specified criteria (usually the unique identifier)
        /// </summary>
        /// <param name="predicate">Criteria to match on</param>
        /// <returns>A single record that matches the specified criteria</returns>
        public T GetSingle(Func<T, bool> predicate)
        {
            return Entities.Single<T>(predicate);
        }

        public T GetSingleAsQueryable(Expression<Func<T, bool>> predicate)
        {
            return Entity.Single<T>(predicate);
        }

        public T GetFirstAsQueryable(Expression<Func<T, bool>> predicate)
        {
            return Entity.FirstOrDefault<T>(predicate);
        }

        /// <summary>
        /// The first record matching the specified criteria
        /// </summary>
        /// <param name="predicate">Criteria to match on</param>
        /// <returns>A single record containing the first record matching the specified criteria</returns>
        public T GetFirst(Func<T, bool> predicate)
        {
            return Entities.First<T>(predicate);
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

        public virtual void Insert(T entity)
        {
            Entities.Add(entity);
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

    }
}
