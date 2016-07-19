using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;

namespace IdentitySample.Models
{
    public class ActionPermissionManager : IDisposable
    {
        private ActionPermissionsStoreBase _actionPermissionStore;

        private ApplicationDbContext _db;

        private bool _disposed;
        private readonly ApplicationRoleManager _roleManager;

        private readonly ApplicationUserManager _userManager;

        public ActionPermissionManager()
        {
            _db = HttpContext.Current
                .GetOwinContext().Get<ApplicationDbContext>();
            _roleManager = HttpContext.Current
               .GetOwinContext().Get<ApplicationRoleManager>();
            _actionPermissionStore = new ActionPermissionsStoreBase(_db);
            _userManager = new ApplicationUserManager(new ApplicationUserStore(_db));
        }

        public IQueryable<ApplicationActionPermission> ActionPermissions
        {
            get
            {
                return _actionPermissionStore.EntitySet;
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
                actionPermission.Roles.Add(role);
            }
            _db.SaveChanges();
            var key = string.Format("{0}/{1}", actionPermission.ControllerName, actionPermission.ActionName);
            WebCache.Remove(key);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> AddPermissionRolesAsync(int actionId, params string[] roles)
        {
            ThrowIfDisposed();
            var actionPermission = _actionPermissionStore.GetById(actionId);
            ClearActionRoles(actionId);

            var newRoles = _roleManager.Roles.Where(r => roles.Any(n => n == r.Name));
            if(actionPermission.Roles == null)
                actionPermission.Roles = new List<ApplicationRole>();

            foreach (var role in newRoles)
            {
                actionPermission.Roles.Add(role);
            }
            await _db.SaveChangesAsync();
            var key = string.Format("{0}/{1}", actionPermission.ControllerName, actionPermission.ActionName);
            WebCache.Remove(key);
            return IdentityResult.Success;
        }

        public bool CreateActionPermission(ApplicationActionPermission permission)
        {
            ThrowIfDisposed();
            if (permission == null)
                throw new ArgumentNullException("permission");

            if (!_actionPermissionStore.EntitySet.Any(x => x.ActionName == permission.ActionName && x.ControllerName == permission.ControllerName))
                _actionPermissionStore.Create(permission);
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
            var perm = GetById(actionId);

            return perm.Roles;
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

        public IEnumerable<ApplicationRole> GetActionRoles(string controller, string action)
        {
            //Check from the cache 1st
            var key = string.Format("{0}/{1}", controller, action);
            var roles = WebCache.Get(key) as List<ApplicationRole>;
            if (roles != null)
                return roles;

            var perm =
                 _db.ApplicationActionPermissions.FirstOrDefault(a => a.ActionName == action & a.ControllerName == controller);

            if (perm == null) return new List<ApplicationRole>();
            roles = GetControllerActionRoles(perm.Id).ToList();
            WebCache.Set(key, roles);
            return roles;
        }

        public bool HasPermission(string controller, string action, string userId)
        {
            var key = string.Format("{0}/{1}/{2}", controller, action, userId);
            var userRoles = WebCache.Get(key) as IList<string>;
            if (userRoles == null)
            {
                userRoles = _userManager.GetRolesAsync(userId).Result;
                WebCache.Set(key, userRoles);
            }
            var requiredRoles = GetActionRoles(controller, action);
            return requiredRoles.Any(x => userRoles.Contains(x.Name));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && _db != null)
            {
                this._db.Dispose();
            }
            this._disposed = true;
            this._db = null;
            this._actionPermissionStore = null;
        }

        private void ClearActionRoles(int actionId)
        {
            var actionPermission = _actionPermissionStore.GetById(actionId);
            if (actionPermission.Roles != null)
            {
                actionPermission.Roles.Clear();
                _db.SaveChanges();
            }
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