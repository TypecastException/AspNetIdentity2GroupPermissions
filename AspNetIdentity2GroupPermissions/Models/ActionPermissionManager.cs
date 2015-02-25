using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace IdentitySample.Models
{
    public class ActionPermissionManager : IDisposable
    {
        private ActionPermissionsStoreBase _actionPermissionStore;

        private ApplicationDbContext _db;

        private bool _disposed;
        private ApplicationRoleManager _roleManager;

        public ActionPermissionManager()
        {
            _db = HttpContext.Current
                .GetOwinContext().Get<ApplicationDbContext>();
            _roleManager = HttpContext.Current
               .GetOwinContext().Get<ApplicationRoleManager>();
            _actionPermissionStore = new ActionPermissionsStoreBase(_db);
        }

        public IQueryable<ApplicationActionPermission> ActionPermissions
        {
            get
            {
                return _actionPermissionStore.EntitySet;
            }
        }

        public IQueryable<ApplicationActionPermissionRole> ActionPermissionsRole
        {
            get
            {
                return _db.ApplicationActionPermissionRoles;
            }
        }

        public IdentityResult AddPermissionRoles(int actionId, params string[] roles)
        {
            ThrowIfDisposed();
            var actionPermission = _actionPermissionStore.GetById(actionId);
            ClearActionRoles(actionId);

            var newRoles = _roleManager.Roles.Where(r => roles.Any(n => n == r.Name));

            foreach (var role in newRoles)
            {
                _db.ApplicationActionPermissionRoles.Add(new ApplicationActionPermissionRole { ActionPermissionId = actionId, RoleId = role.Id });
            }
            _db.SaveChanges();

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> AddPermissionRolesAsync(int actionId, params string[] roles)
        {
            ThrowIfDisposed();
            var actionPermission = _actionPermissionStore.GetById(actionId);
            ClearActionRoles(actionId);

            var newRoles = _roleManager.Roles.Where(r => roles.Any(n => n == r.Name));

            foreach (var role in newRoles)
            {
                _db.ApplicationActionPermissionRoles.Add(new ApplicationActionPermissionRole { ActionPermissionId = actionId, RoleId = role.Id });
            }
            await _db.SaveChangesAsync();

            return IdentityResult.Success;
        }

        public bool CreateActionPermission(ApplicationActionPermission permission)
        {
            ThrowIfDisposed();
            if (permission == null)
                throw new ArgumentNullException("permission");

            if (!_actionPermissionStore.EntitySet.Any(x => x.ActionName == permission.ActionName && x.ControllerName == permission.ControllerName))
                _actionPermissionStore.CreateAsync(permission);
            return true;
        }

        public async Task<bool> CreateActionPermissionAsync(ApplicationActionPermission permission)
        {
            ThrowIfDisposed();
            if (permission == null)
                throw new ArgumentNullException("permission");

            if (!_actionPermissionStore.EntitySet.Any(x => x.ActionName == permission.ActionName && x.ControllerName == permission.ControllerName))
                await _actionPermissionStore.CreateAsync(permission);
            return true;
        }

        public bool Delete(int actionId)
        {
            ThrowIfDisposed();
            var actionPermission = _actionPermissionStore.GetById(actionId);
            if (actionPermission == null)
                throw new ArgumentException("Action Permission not found");
            ClearActionRoles(actionId);
            _actionPermissionStore.Delete(actionPermission);
            return true;
        }

        public async Task<bool> DeleteAsync(int actionId)
        {
            ThrowIfDisposed();
            var actionPermission = await _actionPermissionStore.GetByIdAsync(actionId);
            if (actionPermission == null)
                throw new ArgumentException("Action Permission not found");
            ClearActionRoles(actionId);
            await _actionPermissionStore.DeleteAsync(actionPermission);
            return true;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public ApplicationActionPermission GetById(int id)
        {
            return _actionPermissionStore.GetById(id);
        }

        public Task<ApplicationActionPermission> GetByIdAsync(int id)
        {
            return _actionPermissionStore.GetByIdAsync(id);
        }

        public IEnumerable<ApplicationRole> GetControllerActionRoles(int actionId)
        {
            var perm = _db.ApplicationActionPermissionRoles.Where(x => x.ActionPermissionId == actionId).ToList();
            var roles = _roleManager.Roles.ToList();
            var permRoles = from r in roles
                            where perm.Any(ap => ap.RoleId == r.Id)
                            select r;
            return permRoles;
        }

        public IEnumerable<ApplicationRole> GetControllerActionRoles()
        {
            var perm = _db.ApplicationActionPermissionRoles.ToList();
            var roles = _roleManager.Roles.ToList();
            var permRoles = from r in roles
                            where perm.Any(ap => ap.RoleId == r.Id)
                            select r;
            return permRoles;
        }

        public List<ApplicationActionPermission> GetPermissionRoles()
        {
            var result = _db.ApplicationActionPermissions.ToList();
            result.ForEach(p =>
            {
                p.Roles = _db.ApplicationActionPermissionRoles.Where(x => x.ActionPermissionId == p.Id).ToList();
            });
            return result;
        }

        public bool UpdateActionPermission(ApplicationActionPermission permission)
        {
            ThrowIfDisposed();
            if (permission == null)
                throw new ArgumentNullException("permission");

            _actionPermissionStore.Update(permission);
            return true;
        }

        public async Task<bool> UpdateActionPermissionAsync(ApplicationActionPermission permission)
        {
            ThrowIfDisposed();
            if (permission == null)
                throw new ArgumentNullException("permission");

            await _actionPermissionStore.UpdateAsync(permission);
            return true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && this._db != null)
            {
                this._db.Dispose();
            }
            this._disposed = true;
            this._db = null;
            this._actionPermissionStore = null;
        }

        private void ClearActionRoles(int actionId)
        {
            _db.ApplicationActionPermissionRoles.Where(x => x.ActionPermissionId == actionId).ToList().
                  ForEach(x => _db.ApplicationActionPermissionRoles.Remove(x));
            _db.SaveChanges();
        }

        private void ThrowIfDisposed()
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }
    }
}