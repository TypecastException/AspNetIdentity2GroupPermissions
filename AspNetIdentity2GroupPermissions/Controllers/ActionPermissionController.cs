using IdentitySample.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IdentitySample.Controllers
{
    public class ActionPermissionController : Controller
    {
        private ActionPermissionManager _actionPermissionManager;
        private ApplicationRoleManager _roleManager;

        public ActionPermissionManager ActionPermissionManager
        {
            get
            {
                return _actionPermissionManager ?? new ActionPermissionManager();
            }
            private set
            {
                _actionPermissionManager = value;
            }
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext()
                    .Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        public ActionResult Create()
        {
            ViewBag.RolesList = new SelectList(
                  this.RoleManager.Roles.ToList(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ControllerName,ActionName")] ApplicationActionPermission applicationgroup, params string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                var result = await ActionPermissionManager.CreateActionPermissionAsync(applicationgroup);
                if (result)
                    await ActionPermissionManager.AddPermissionRolesAsync(applicationgroup.Id, selectedRoles);
                return RedirectToAction("Index");
            }
            ViewBag.RolesList = new SelectList(
                  this.RoleManager.Roles.ToList(), "Id", "Name");
            return View(applicationgroup);
        }

        [ActionAuthorize]
        public async Task<ActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var result = await this.ActionPermissionManager.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Details(string controllerName)
        {
            if (string.IsNullOrEmpty(controllerName))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var applicationpermissions =
                 this.ActionPermissionManager.ActionPermissions.Where(x => x.ControllerName == controllerName).ToList();
            if (!applicationpermissions.Any())
            {
                return HttpNotFound();
            }

            return View(applicationpermissions);
        }

        public async Task<ActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var applicationpermissions =
                await this.ActionPermissionManager.GetByIdAsync(id);
            if (applicationpermissions == null)
            {
                return HttpNotFound();
            }
            var groupRoles = this.ActionPermissionManager.GetControllerActionRoles(id);
            // Get a list, not a DbSet or queryable:
            var allRoles = this.RoleManager.Roles.ToList();

            var model = new PermissionViewModel()
            {
                Id = applicationpermissions.Id,
                ControllerName = applicationpermissions.ControllerName,
                ActionName = applicationpermissions.ActionName
            };

            // load the roles/Roles for selection in the form:
            foreach (var Role in allRoles)
            {
                var listItem = new SelectListItem()
                {
                    Text = Role.Name,
                    Value = Role.Id,
                    Selected = groupRoles.Any(g => g.Id == Role.Id)
                };
                model.RolesList.Add(listItem);
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(
            [Bind(Include = "Id,ControllerName,ActionName")] PermissionViewModel model, params string[] selectedRoles)
        {
            var permission = await this.ActionPermissionManager.GetByIdAsync(model.Id);
            if (permission == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                permission.ActionName = model.ActionName;
                permission.ControllerName = model.ControllerName;
                await this.ActionPermissionManager.UpdateActionPermissionAsync(permission);

                selectedRoles = selectedRoles ?? new string[] { };
                await this.ActionPermissionManager.AddPermissionRolesAsync(permission.Id, selectedRoles);
                return RedirectToAction("Permissions", new { id = permission.Id });
            }
            return View(model);
        }

        // GET: ActionPermission

        public ActionResult Index()
        {
            return View(ActionPermissionManager.ActionPermissions.ToList());
        }

        public async Task<ActionResult> Permissions(int id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var applicationpermission =
                await this.ActionPermissionManager.GetByIdAsync(id);
            if (applicationpermission == null)
            {
                return HttpNotFound();
            }
            var groupRoles = this.ActionPermissionManager.GetControllerActionRoles(id);
            string[] RoleNames = groupRoles.Select(p => p.Name).ToArray();
            ViewBag.RolesList = RoleNames;

            return View(applicationpermission);
        }
    }
}