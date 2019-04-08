using NetworkHairdressing.Models;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace NetworkHairdressing.Controllers
{
    public class HomeController : Controller
    {
        NetworkHairdressingContext _db = new NetworkHairdressingContext();

        public ActionResult Index()
        {
            return View(_db.Barbershops);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Developer by Ganevich Andrey.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact page.";

            return View();
        }

        public ActionResult Info()
        {
            ViewBag.Message = "Information";

            return View();
        }

        public ActionResult Price()
        {
            ViewBag.Message = "Price";

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}