
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
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

        /// <summary>
        /// Join the multiple table using InnerJoin Method
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="predicate">if no filter o => true or c => !c.IsDeleted</param>
        /// <param name="includeFunc"></param>
        /// <param name="joinSelector">Join the Multiple table</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IQueryable<TResult> InnerJoin<TResult>(
         Expression<Func<T, bool>> predicate,               
         Func<IQueryable<T>, IQueryable<T>> includeFunc = null,
         Func<IQueryable<T>, IQueryable<TResult>>? joinSelector = null)
        {
            IQueryable<T> query = Entities.AsNoTracking();

            if (includeFunc != null)
                query = includeFunc(query);

            query = query.Where(predicate);

            if (joinSelector != null)
                return joinSelector(query);

            if (typeof(TResult) == typeof(T))
                return (IQueryable<TResult>)query;

            throw new NotImplementedException("Join selector required when TResult != T.");


        }

        /// <summary>
        /// Insert and Update table using Upsert Method
        /// </summary>
        /// <param name="entity">Create Enitity Model and Map</param>
        /// <param name="predicate"> e => e.Id == myEntity.Id</param>
        public async Task Upsert(T entity, Expression<Func<T, bool>> predicate)
        {
            ArgumentNullException.ThrowIfNull(entity, "entity");
            ArgumentNullException.ThrowIfNull(predicate, "predicate");
            T existingEntity = await Entities.FirstOrDefaultAsync(predicate);
            DateTime now = DateTime.Now;
            IEntityType entityType = context.Model.FindEntityType(typeof(T)) ?? throw new InvalidOperationException("Entity type " + typeof(T).Name + " not found in the current DbContext model");
            PropertyInfo createdAt = typeof(T).GetProperty("CreatedAt");
            PropertyInfo updatedAt = typeof(T).GetProperty("UpdatedAt");

            HashSet<string> concurrentProps = (from p in entityType.GetProperties()
                                               where p.IsConcurrencyToken
                                               select p.Name).ToHashSet();
            if (existingEntity != null)
            {
                EntityEntry<T> entry = context.Entry(existingEntity);
                if (updatedAt != null)
                {
                    updatedAt.SetValue(existingEntity, now);
                    entry.Property(updatedAt.Name).IsModified = true;
                }

                foreach (PropertyEntry property in entry.Properties)
                {
                    if (!property.Metadata.IsKey() && !concurrentProps.Contains(property.Metadata.Name) && !(property.Metadata.Name == updatedAt?.Name))
                    {
                        object newValue = typeof(T).GetProperty(property.Metadata.Name)?.GetValue(entity);
                        object currentValue = property.CurrentValue;
                        if (!object.Equals(newValue, currentValue))
                        {
                            property.CurrentValue = newValue;
                            property.IsModified = true;
                        }
                    }
                }
            }
            else
            {
                if (createdAt != null && updatedAt != null)
                {
                    createdAt.SetValue(entity, now);
                    updatedAt.SetValue(entity, now);
                }

                await Entities.AddAsync(entity);
            }
        }

    }
}
