using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IdentitySample.Models
{
    public class ApplicationActionPermission
    {
        public ApplicationActionPermission()
        {
        }

        public ApplicationActionPermission(string actionName, string controllerName)
            : this()
        {
            this.ActionName = actionName;
            this.ControllerName = controllerName;
            this.Roles = new Collection<ApplicationActionPermissionRole>();
        }

        public virtual string ActionName { get; set; }

        public virtual string ControllerName { get; set; }

        [Key]
        [Required]
        public virtual int Id { get; set; }

        public virtual ICollection<ApplicationActionPermissionRole> Roles { get; set; }
    }
}