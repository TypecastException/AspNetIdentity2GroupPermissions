using IdentitySample.Models;
using System.Web.Mvc;

namespace IdentitySample.Controllers
{
    [PermissionFilter]
    public class SecureController : Controller
    {
        // GET: Secure
        public ActionResult Index()
        {
            return View();
        }

        // GET: Secure/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Secure/Create
        public ActionResult Create()
        {
            return View();
        }
    }
}