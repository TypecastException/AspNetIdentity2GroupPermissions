using IdentitySample.Models;
using Microsoft.AspNet.Identity.Owin;
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
        public async Task<ActionResult> Create([Bind(Include = "ControllerName,ActionName")] ApplicationActionPermission applicationpermission, params string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                selectedRoles = selectedRoles ?? new string[] { };
                var result = await ActionPermissionManager.CreateActionPermissionAsync(applicationpermission);
                if (result)
                    await ActionPermissionManager.AddPermissionRolesAsync(applicationpermission.Id, selectedRoles);
                return RedirectToAction("Index");
            }
            ViewBag.RolesList = new SelectList(
                  this.RoleManager.Roles.ToList(), "Id", "Name");
            return View(applicationpermission);
        }

        [PermissionFilter]
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
            var actionRoles = this.ActionPermissionManager.GetControllerActionRoles(id).ToList();
            // Get a list, not a DbSet or queryable:
            var allRoles = this.RoleManager.Roles.ToList();

            var model = new PermissionViewModel()
            {
                Id = applicationpermissions.Id,
                ControllerName = applicationpermissions.ControllerName,
                ActionName = applicationpermissions.ActionName
            };

            // load the roles/Roles for selection in the form:
            foreach (var role in allRoles)
            {
                var listItem = new SelectListItem()
                {
                    Text = role.Name,
                    Value = role.Id,
                    Selected = actionRoles.Any(g => g.Id == role.Id)
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
            var model =
                ActionPermissionManager.ActionPermissions.GroupBy(a => a.ControllerName).Select(g => new ActionViewModel { Actions = g.Count(), Controller = g.Key }).ToList();

            return View(model);
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
            var actionRoles = this.ActionPermissionManager.GetControllerActionRoles(id);
            string[] roleNames = actionRoles.Select(p => p.Name).ToArray();
            ViewBag.RolesList = roleNames;

            return View(applicationpermission);
        }
    }
}