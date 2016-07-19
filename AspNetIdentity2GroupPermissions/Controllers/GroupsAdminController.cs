using IdentitySample.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IdentitySample.Controllers
{
    [Authorize(Roles = "Admin")]
    public class GroupsAdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private ApplicationGroupManager _groupManager;

        public ApplicationGroupManager GroupManager
        {
            get
            {
                return _groupManager ?? new ApplicationGroupManager();
            }
            private set
            {
                _groupManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;

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

        [PermissionFilter]
        public ActionResult Index()
        {
            return View(this.GroupManager.Groups.ToList());
        }

        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationGroup applicationgroup =
                await this.GroupManager.Groups.FirstOrDefaultAsync(g => g.Id == id);
            if (applicationgroup == null)
            {
                return HttpNotFound();
            }
            var groupRoles = this.GroupManager.GetGroupRoles(applicationgroup.Id);
            string[] RoleNames = groupRoles.Select(p => p.Name).ToArray();
            ViewBag.RolesList = RoleNames;
            ViewBag.RolesCount = RoleNames.Count();
            return View(applicationgroup);
        }

        public ActionResult Create()
        {
            //Get a SelectList of Roles to choose from in the View:
            ViewBag.RolesList = new SelectList(
                this.RoleManager.Roles.ToList(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
            [Bind(Include = "Name,Description")] ApplicationGroup applicationgroup, params string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                // Create the new Group:
                var result = await this.GroupManager.CreateGroupAsync(applicationgroup);
                if (result.Succeeded)
                {
                    selectedRoles = selectedRoles ?? new string[] { };

                    // Add the roles selected:
                    await this.GroupManager.SetGroupRolesAsync(applicationgroup.Id, selectedRoles);
                }
                return RedirectToAction("Index");
            }

            // Otherwise, start over:
            ViewBag.RoleId = new SelectList(
                this.RoleManager.Roles.ToList(), "Id", "Name");
            return View(applicationgroup);
        }

        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationGroup applicationgroup = await this.GroupManager.FindByIdAsync(id);
            if (applicationgroup == null)
            {
                return HttpNotFound();
            }

            // Get a list, not a DbSet or queryable:
            var allRoles = await this.RoleManager.Roles.ToListAsync();
            var groupRoles = await this.GroupManager.GetGroupRolesAsync(id);

            var model = new GroupViewModel()
            {
                Id = applicationgroup.Id,
                Name = applicationgroup.Name,
                Description = applicationgroup.Description,
                IsAdmin = applicationgroup.IsAdmin
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
            [Bind(Include = "Id,Name,Description,IsAdmin")] GroupViewModel model, params string[] selectedRoles)
        {
            var group = await this.GroupManager.FindByIdAsync(model.Id);
            if (group == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                group.Name = model.Name;
                group.Description = model.Description;
                await this.GroupManager.UpdateGroupAsync(group);

                selectedRoles = selectedRoles ?? new string[] { };
                await this.GroupManager.SetGroupRolesAsync(group.Id, selectedRoles);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationGroup applicationgroup = await this.GroupManager.FindByIdAsync(id);
            if (applicationgroup == null)
            {
                return HttpNotFound();
            }
            return View(applicationgroup);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationGroup applicationgroup = await this.GroupManager.FindByIdAsync(id);
            await this.GroupManager.DeleteGroupAsync(id);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}