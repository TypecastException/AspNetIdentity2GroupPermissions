namespace IdentitySample.Migrations
{
    using IdentitySample.Models;
    using Microsoft.AspNet.Identity;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "IdentitySample.Models.ApplicationDbContext";
        }

        protected override void Seed(ApplicationDbContext context)
        {
            context.ApplicationActionPermissions.AddOrUpdate(new ApplicationActionPermission { ActionName = "Create", ControllerName = "ActionPermissionController", Id = 1 });
            var roleManager = new ApplicationRoleManager(new ApplicationRoleStore(context));
            var userManager = new ApplicationUserManager(new ApplicationUserStore(context));

            const string name = "admin@example.com";
            const string password = "Admin@123456";
            const string roleName = "Admin";

            //Create Role Admin if it does not exist
            var role = roleManager.FindByName(roleName);
            if (role == null)
            {
                role = new ApplicationRole(roleName);
                var roleresult = roleManager.Create(role);
            }

            var user = userManager.FindByName(name);
            if (user == null)
            {
                user = new ApplicationUser { UserName = name, Email = name, EmailConfirmed = true };
                var result = userManager.Create(user, password);
                result = userManager.SetLockoutEnabled(user.Id, false);
            }

            var groupManager = new ApplicationGroupManager(context, userManager, roleManager);
            var newGroup = new ApplicationGroup("SuperAdmins", "Full Access to All") { IsAdmin = true };

            groupManager.CreateGroup(newGroup);
            groupManager.SetUserGroups(user.Id, new string[] { newGroup.Id });
            groupManager.SetGroupRoles(newGroup.Id, new string[] { role.Name });
        }
    }
}