using NetworkHairdressing.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace NetworkHairdressing.Controllers
{
    [Authorize(Roles = "admin")]
    public class TimeSheetsController : Controller
    {
        private NetworkHairdressingContext db = new NetworkHairdressingContext();

        // GET: TimeSheets
        public ActionResult Index()
        {
            return View(db.TimeSheets.ToList());
        }

        public ActionResult AutoCreateTimeSheet()
        {
            var currendDate = DateTime.Now;
            var days = DateTime.DaysInMonth(currendDate.Year, currendDate.Month);

            var previewDate = currendDate.AddMonths(-1);
            var previewName = string.Join(string.Empty, previewDate.Month.ToString(), "-",
                previewDate.Year.ToString(), ".tmp");

            var previewTimeSheet = db.TimeSheets.FirstOrDefault(x => x.Name.Contains(previewName));
            var employeeDictionary = new Dictionary<Employee, int>();

            if (previewTimeSheet != null)
            {
                var previewFile = previewTimeSheet.File;
                var previewTempFileName = Path.GetTempFileName();

                using (var stream = new FileStream(previewTempFileName, FileMode.Create))
                {
                    stream.Write(previewFile, 0, previewFile.Length);
                }
                
                using (var sr = new StreamReader(previewTempFileName, Encoding.GetEncoding(1251)))
                {
                    var line = sr.ReadLine();
                    while ((line = sr.ReadLine()) != null)
                    {
                        var lengthEmployeeId = line.IndexOf(';');
                        var preEmployeeId = int.Parse(line.Substring(0, lengthEmployeeId));
                        var preEmployee = db.Employees.First(x => x.Id == preEmployeeId);

                        if (preEmployee.BarbershopId != 3)
                        {
                            continue;
                        }

                        var preCounter = 0;
                        var preLastDay = line.Substring(line.Length - 4, 1);
                        var lastDay = line.Substring(line.Length - 2, 1);

                        switch (preLastDay)
                        {
                            case "в" when lastDay == "р":
                                preCounter = 1;
                                break;
                            case "р" when lastDay == "в":
                                preCounter = -1;
                                break;
                            case "р" when lastDay == "р":
                                preCounter = -2;
                                break;
                        }

                        employeeDictionary.Add(preEmployee, preCounter);
                    }
                }
            }

            var path = Path.GetTempFileName();

            var file = new FileStream(path, FileMode.Append);
            var streamWriter = new StreamWriter(file, Encoding.GetEncoding(1251));

            streamWriter.Write(";");
            for (var i = 1; i <= days; i++)
            {
                streamWriter.Write(string.Join(string.Empty, i, ";"));
            }

            streamWriter.WriteLine();

            foreach (var employee in db.Employees.OrderBy(x => x.BarbershopId))
            {
                streamWriter.Write(string.Join(string.Empty, employee.Id, ";"));

                var counter = 0;
                if (employeeDictionary.ContainsKey(employee))
                {
                    counter = employeeDictionary[employee];
                }

                for (var i = 0; i < days; i++)
                {
                    var dayDate = new DateTime(currendDate.Year, currendDate.Month, i + 1);
                    if (employee.BarbershopId != 3)
                    {
                        streamWriter.Write(dayDate.DayOfWeek == DayOfWeek.Sunday
                            ? string.Join(string.Empty, "в", ";")
                            : string.Join(string.Empty, "р", ";"));
                    }
                    else
                    {
                        if (counter < 2 && counter >= 0)
                        {
                            streamWriter.Write(string.Join(string.Empty, "р", ";"));
                        }
                        else
                        {
                            streamWriter.Write(string.Join(string.Empty, "в", ";"));
                            if (counter != -1)
                            {
                                counter = -2;
                            }
                        }
                    }

                    counter++;
                }

                streamWriter.WriteLine();
            }
            streamWriter.Close();

            var timeSheetName = string.Join(string.Empty, currendDate.Month.ToString(), "-",
                currendDate.Year.ToString(), ".tmp");
            var timeSheet = db.TimeSheets.FirstOrDefault(x => x.Name.Contains(timeSheetName));

            file = new FileStream(path, FileMode.Open);
            byte[] fileByte = null;
            using (var binaryReader = new BinaryReader(file))
            {
                fileByte = binaryReader.ReadBytes((int)file.Length);
            }

            if (timeSheet != null)
            {
                timeSheet.File = fileByte;
            }
            else
            {
                timeSheet = new TimeSheet
                {
                    Name = timeSheetName,
                    File = fileByte
                };
                db.TimeSheets.Add(timeSheet);
            }

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: TimeSheets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var timeSheet = db.TimeSheets.Find(id);
            if (timeSheet == null)
            {
                return HttpNotFound();
            }

            var timeSheetFile = timeSheet.File;
            var tempFileName = Path.GetTempFileName();

            using (var stream = new FileStream(tempFileName, FileMode.Create))
            {
                stream.Write(timeSheetFile, 0, timeSheetFile.Length);
            }

            var numberOfDays = 0;
            var text = new List<Dictionary<Employee, List<JobStatus>>>();
            using (var sr = new StreamReader(tempFileName, Encoding.GetEncoding(1251)))
            {
                var line = sr.ReadLine();
                while ((line = sr.ReadLine()) != null)
                {
                    numberOfDays = line.Split(';').Length - 1;

                    var strEmployeeId = line.Substring(0, line.IndexOf(';'));
                    var employeeId = int.Parse(strEmployeeId);
                    var employee = db.Employees.First(x => x.Id == employeeId);
                    line = line.Replace(string.Join(string.Empty, strEmployeeId, ";"), string.Empty);

                    var listJobStatus = new List<JobStatus>();
                    foreach (var jobStatus in line.Split(';'))
                    {
                        Enum.TryParse(jobStatus, out JobStatus jS);
                        listJobStatus.Add(jS);
                    }

                    var dictionary = new Dictionary<Employee, List<JobStatus>> {{employee, new List<JobStatus>(listJobStatus)}};
                    text.Add(new Dictionary<Employee, List<JobStatus>>(dictionary));

                    listJobStatus.Clear();
                    dictionary.Clear();
                }
            }
            
            ViewBag.JobStatus = Enum.GetValues(typeof(JobStatus)).Cast<JobStatus>();
            var timeSheetView = new TimeSheetView(timeSheet, text, numberOfDays);

            return View(timeSheetView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(TimeSheet timeSheet, int[] employees, string[] jobStatuses)
        {
            timeSheet = db.TimeSheets.First(x => x.Id == timeSheet.Id);

            var stringTimeSheetYear =
                timeSheet.Name.Substring(timeSheet.Name.IndexOf('-') + 1, 4);
            var timeSheetYear = int.Parse(stringTimeSheetYear);

            var timeSheetMonth = int.Parse(timeSheet.Name.Substring(0, timeSheet.Name.IndexOf('-')));
            var days = DateTime.DaysInMonth(timeSheetYear, timeSheetMonth);

            var path = Path.GetTempFileName();

            var file = new FileStream(path, FileMode.Append);
            var streamWriter = new StreamWriter(file, Encoding.GetEncoding(1251));

            streamWriter.Write(";");
            for (var i = 1; i <= days; i++)
            {
                streamWriter.Write(string.Join(string.Empty, i, ";"));
            }

            streamWriter.WriteLine();

            for (var i = 0; i < employees.Length; i++)
            {
                streamWriter.Write(string.Join(string.Empty, employees[i], ";"));
                
                for (var j = 0 + (i * days); j < (1 + i) * days; j++)
                {
                    streamWriter.Write(string.Join(string.Empty, jobStatuses[j], ";"));
                }

                streamWriter.WriteLine();
            }
            streamWriter.Close();

            file = new FileStream(path, FileMode.Open);
            byte[] fileByte = null;
            using (var binaryReader = new BinaryReader(file))
            {
                fileByte = binaryReader.ReadBytes((int)file.Length);
            }

            timeSheet.File = fileByte;

            db.SaveChanges();

            return RedirectToAction("Details");
        }

        // GET: TimeSheets/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TimeSheets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id")] TimeSheet timeSheet, HttpPostedFileBase upimage = null)
        {
            if (ModelState.IsValid)
            {
                if (upimage != null)
                {
                    byte[] imageData = null;
                    using (var binaryReader = new BinaryReader(upimage.InputStream))
                    {
                        imageData = binaryReader.ReadBytes(upimage.ContentLength);
                    }

                    timeSheet.File = imageData;
                }

                var currendDate = DateTime.Now;
                timeSheet.Name = string.Join(string.Empty, currendDate.Month.ToString(), currendDate.Year.ToString(), ".tmp");

                db.TimeSheets.Add(timeSheet);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(timeSheet);
        }

        // GET: TimeSheets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TimeSheet timeSheet = db.TimeSheets.Find(id);
            if (timeSheet == null)
            {
                return HttpNotFound();
            }
            return View(timeSheet);
        }

        // POST: TimeSheets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,File")] TimeSheet timeSheet)
        {
            if (ModelState.IsValid)
            {
                db.Entry(timeSheet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(timeSheet);
        }

        // GET: TimeSheets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TimeSheet timeSheet = db.TimeSheets.Find(id);
            if (timeSheet == null)
            {
                return HttpNotFound();
            }
            return View(timeSheet);
        }

        // POST: TimeSheets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TimeSheet timeSheet = db.TimeSheets.Find(id);
            db.TimeSheets.Remove(timeSheet);
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
