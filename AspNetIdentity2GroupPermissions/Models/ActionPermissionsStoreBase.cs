using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace IdentitySample.Models
{
    /// TODO : Convert to generic of T
    public class ActionPermissionsStoreBase
    {
        public ActionPermissionsStoreBase(DbContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            this.Context = context;
            this.DbEntitySet = context.Set<ApplicationActionPermission>();
        }

        public DbContext Context
        {
            get;
            private set;
        }

        public DbSet<ApplicationActionPermission> DbEntitySet
        {
            get;
            private set;
        }

        public IQueryable<ApplicationActionPermission> EntitySet
        {
            get
            {
                return this.DbEntitySet;
            }
        }

        public virtual void Create(ApplicationActionPermission entity)
        {
            this.DbEntitySet.Add(entity);
            this.Context.SaveChanges();
        }

        public virtual Task CreateAsync(ApplicationActionPermission entity)
        {
            this.DbEntitySet.Add(entity);
            return this.Context.SaveChangesAsync();
        }

        public void Delete(ApplicationActionPermission entity)
        {
            this.DbEntitySet.Remove(entity);
            this.Context.SaveChanges();
        }

        public virtual Task DeleteAsync(ApplicationActionPermission entity)
        {
            this.DbEntitySet.Remove(entity);
            return this.Context.SaveChangesAsync();
        }

        public virtual ApplicationActionPermission GetById(object id)
        {
            return this.DbEntitySet.Find(new object[] { id });
        }

        public virtual Task<ApplicationActionPermission> GetByIdAsync(object id)
        {
            return this.DbEntitySet.FindAsync(new object[] { id });
        }

        public virtual void Update(ApplicationActionPermission entity)
        {
            if (entity != null)
            {
                this.Context.Entry<ApplicationActionPermission>(entity).State = EntityState.Modified;
            }
            this.Context.SaveChanges();
        }

        public virtual Task UpdateAsync(ApplicationActionPermission entity)
        {
            if (entity != null)
            {
                this.Context.Entry<ApplicationActionPermission>(entity).State = EntityState.Modified;
            }
            return this.Context.SaveChangesAsync();
        }
    }
}