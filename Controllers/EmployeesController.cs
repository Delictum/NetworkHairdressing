using NetworkHairdressing.Models;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace NetworkHairdressing.Controllers
{
    public class EmployeesController : Controller
    {
        private NetworkHairdressingContext db = new NetworkHairdressingContext();

        // GET: Employees
        public ActionResult Index()
        {
            return View(db.Employees.ToList());
        }

        // GET: Employees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // GET: Employees/Create
        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult Create(Employee employee, HttpPostedFileBase upimage = null)
        {
            if (!ModelState.IsValid)
            {
                return View(employee);
            }

            if (upimage != null)
            {
                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(upimage.InputStream))
                {
                    imageData = binaryReader.ReadBytes(upimage.ContentLength);
                }

                employee.Image = imageData;
            }

            db.Employees.Add(employee);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Employees/Edit/5
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult Edit(Employee employee, HttpPostedFileBase upimage = null)
        {
            if (!ModelState.IsValid)
            {
                return View(employee);
            }

            if (upimage != null)
            {
                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(upimage.InputStream))
                {
                    imageData = binaryReader.ReadBytes(upimage.ContentLength);
                }

                employee.Image = imageData;
            }

            db.Entry(employee).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Employees/Delete/5
        [Authorize(Roles = "admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Employee employee = db.Employees.Find(id);
            db.Employees.Remove(employee);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult EmployeesSearch(string id)
        {
            var allEmployees = db.Employees.Where(a => a.Fullname == id).ToList();
            return PartialView(allEmployees);
        }

        [HttpGet]
        public ActionResult EmployeesSearch(int? id)
        {
            var allEmployees = db.Employees.Where(a => a.BarbershopId == id).ToList();
            return View(allEmployees);
        }

        [HttpGet]
        public ActionResult EmployeeWorksSearch(int? id)
        {
            var allEmployees = db.Employees.Where(a => a.Id == id).ToList();
            return View(allEmployees);
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
