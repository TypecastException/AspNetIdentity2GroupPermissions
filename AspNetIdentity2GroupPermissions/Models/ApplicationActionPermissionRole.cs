namespace IdentitySample.Models
{
    public class ApplicationActionPermissionRole
    {
        public virtual int PermissionId { get; set; }

        public virtual ApplicationActionPermission Permission { get; set; }

        public virtual ApplicationRole Role { get; set; }

        public virtual string RoleId { get; set; }
    }
}