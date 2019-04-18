using NetworkHairdressing.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace NetworkHairdressing.Controllers
{
    public class BarbershopsController : Controller
    {
        private NetworkHairdressingContext db = new NetworkHairdressingContext();

        // GET: Barbershops
        public ActionResult Index()
        {
            return View(db.Barbershops.ToList());
        }

        // GET: Barbershops/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Barbershop barbershop = db.Barbershops.Find(id);
            if (barbershop == null)
            {
                return HttpNotFound();
            }
            return View(barbershop);
        }

        // GET: Barbershops/Create
        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Barbershops/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult Create([Bind(Include = "Id,Name")] Barbershop barbershop)
        {
            if (ModelState.IsValid)
            {
                db.Barbershops.Add(barbershop);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(barbershop);
        }

        // GET: Barbershops/Edit/5
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Barbershop barbershop = db.Barbershops.Find(id);
            if (barbershop == null)
            {
                return HttpNotFound();
            }
            return View(barbershop);
        }

        // POST: Barbershops/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult Edit([Bind(Include = "Id,Name")] Barbershop barbershop)
        {
            if (ModelState.IsValid)
            {
                db.Entry(barbershop).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(barbershop);
        }

        // GET: Barbershops/Delete/5
        [Authorize(Roles = "admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Barbershop barbershop = db.Barbershops.Find(id);
            if (barbershop == null)
            {
                return HttpNotFound();
            }
            return View(barbershop);
        }

        // POST: Barbershops/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Barbershop barbershop = db.Barbershops.Find(id);
            db.Barbershops.Remove(barbershop);
            db.SaveChanges();
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
