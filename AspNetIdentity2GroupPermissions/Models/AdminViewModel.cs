using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace IdentitySample.Models
{
    public class EditUserViewModel
    {
        public EditUserViewModel()
        {
            this.RolesList = new List<SelectListItem>();
            this.GroupsList = new List<SelectListItem>();
        }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        // Add a GroupsList Property:
        public ICollection<SelectListItem> GroupsList { get; set; }

        public string Id { get; set; }

        // We will still use this, so leave it here:
        public ICollection<SelectListItem> RolesList { get; set; }
    }

    public class GroupViewModel
    {
        public GroupViewModel()
        {
            this.UsersList = new List<SelectListItem>();
            this.RolesList = new List<SelectListItem>();
        }

        public string Description { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        public bool IsAdmin { get; set; }

        public ICollection<SelectListItem> RolesList { get; set; }

        public ICollection<SelectListItem> UsersList { get; set; }
    }

    public class PermissionViewModel
    {
        public PermissionViewModel()
        {
            this.RolesList = new List<SelectListItem>();
        }

        [Required(AllowEmptyStrings = false)]
        public string ActionName { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string ControllerName { get; set; }

        [Required]
        public int Id { get; set; }

        public ICollection<SelectListItem> RolesList { get; set; }
    }

    public class RoleViewModel
    {
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "RoleName")]
        public string Name { get; set; }
    }

    public class ActionViewModel
    {
        public int Id { get; set; }

        public string Controller { get; set; }

        public int Actions { get; set; }
    }
}