using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IdentitySample.Models
{
    public class ApplicationActionPermissionRole
    {
        public virtual int ActionPermissionId { get; set; }

        public virtual ApplicationActionPermission Permission { get; set; }

        public virtual ApplicationRole Role { get; set; }

        public virtual string RoleId { get; set; }
    }
}