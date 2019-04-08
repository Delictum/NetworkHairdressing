using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NetworkHairdressing.Models;

namespace NetworkHairdressing.Controllers
{
    public class EmployeeWorksController : Controller
    {
        private NetworkHairdressingContext db = new NetworkHairdressingContext();

        // GET: EmployeeWorks
        public ActionResult Index()
        {
            return View(db.EmployeeWorks.ToList());
        }

        // GET: EmployeeWorks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeeWork employeeWork = db.EmployeeWorks.Find(id);
            if (employeeWork == null)
            {
                return HttpNotFound();
            }
            return View(employeeWork);
        }
        
        // GET: EmployeeWorks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EmployeeWorks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EmployeeWork employeeWork, HttpPostedFileBase upimage = null)
        {
            if (!ModelState.IsValid)
            {
                return View(employeeWork);
            }

            if (upimage != null)
            {
                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(upimage.InputStream))
                {
                    imageData = binaryReader.ReadBytes(upimage.ContentLength);
                }

                employeeWork.Image = imageData;
            }

            db.EmployeeWorks.Add(employeeWork);
            db.SaveChanges();
            return RedirectToAction("Index");

        }

        // GET: EmployeeWorks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeeWork employeeWork = db.EmployeeWorks.Find(id);
            if (employeeWork == null)
            {
                return HttpNotFound();
            }
            return View(employeeWork);
        }

        // POST: EmployeeWorks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EmployeeWork employeeWork, HttpPostedFileBase upimage = null)
        {
            if (!ModelState.IsValid)
            {
                return View(employeeWork);
            }

            if (upimage != null)
            {
                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(upimage.InputStream))
                {
                    imageData = binaryReader.ReadBytes(upimage.ContentLength);
                }

                employeeWork.Image = imageData;
            }

            db.Entry(employeeWork).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: EmployeeWorks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeeWork employeeWork = db.EmployeeWorks.Find(id);
            if (employeeWork == null)
            {
                return HttpNotFound();
            }
            return View(employeeWork);
        }

        // POST: EmployeeWorks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EmployeeWork employeeWork = db.EmployeeWorks.Find(id);
            db.EmployeeWorks.Remove(employeeWork);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult EmployeeWorksSearch(int? id)
        {
            var allEmployeeWorks = db.EmployeeWorks.Where(a => a.EmployeeId == id).ToList();
            return View(allEmployeeWorks);
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
