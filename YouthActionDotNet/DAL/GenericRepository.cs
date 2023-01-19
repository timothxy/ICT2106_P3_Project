using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YouthActionDotNet.Data;

namespace YouthActionDotNet.DAL
{
    public class GenericRepository<TEntity> where TEntity: class{
        internal DBContext context;
        internal DbSet<TEntity> dbSet;
        
        public GenericRepository(DBContext context){
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }
        
        public virtual IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public virtual TEntity GetByID(object id)
        {
            return dbSet.Find(id);
        }

        public virtual bool Insert(TEntity entity)
        {
            try{
                dbSet.Add(entity);
                return true;
            }catch{
                return false;
            }
        }

        public virtual bool Update(TEntity entityToUpdate)
        {
            try{
                dbSet.Attach(entityToUpdate);
                context.Entry(entityToUpdate).State = EntityState.Modified;
                return true;
            }catch{
                return false;
            }
        }

        public virtual bool Delete(object id)
        {
            try{
                TEntity entityToDelete = dbSet.Find(id);
                Delete(entityToDelete);
                return true;
            }catch{
                return false;
            }
        }

        public virtual bool Delete(TEntity entityToDelete)
        {
            try{
                if (context.Entry(entityToDelete).State == EntityState.Detached)
                {
                    dbSet.Attach(entityToDelete);
                }
                dbSet.Remove(entityToDelete);
                return true;
            }catch{
                return false;
            }
        }



        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }
            else
            {
                return await query.ToListAsync();
            }
        }

        public virtual async Task<TEntity> GetByIDAsync(object id)
        {
            return await dbSet.FindAsync(id);
        }
        

        public virtual async Task<bool> InsertAsync(TEntity entity)
        {
            try{
                await dbSet.AddAsync(entity);
                await context.SaveChangesAsync();
                return true;
            }catch{
                return false;
            }
        }

        public virtual async Task<bool> UpdateAsync(TEntity entityToUpdate)
        {
            try{
                dbSet.Attach(entityToUpdate);
                context.Entry(entityToUpdate).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return true;
            }catch{
                return false;
            }
        }

        public virtual async Task<bool> DeleteAsync(object id)
        {
            try{
                TEntity entityToDelete = await dbSet.FindAsync(id);
                return await DeleteAsync(entityToDelete);
            }catch{
                return false;
            }
        }

        public virtual async Task<bool> DeleteAsync(TEntity entityToDelete)
        {
            try{
                if (context.Entry(entityToDelete).State == EntityState.Detached)
                {
                    dbSet.Attach(entityToDelete);
                }
                dbSet.Remove(entityToDelete);
                await context.SaveChangesAsync();
                return true;
            }catch{
                return false;
            }
        }
    }
}