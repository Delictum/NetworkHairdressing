using System;
using System.Collections.Generic;
using NetworkHairdressing.Models;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

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
            ViewBag.Message = "Developer by Delictum Est.";

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

        private ReceptionViewModels LoadRecordModel()
        {
            const int selectedIndex = 1;

            return new ReceptionViewModels
            {
                ListBarbershops = new SelectList(_db.Barbershops, "Id", "Name", selectedIndex),
                ListEmployees = new SelectList(_db.Employees.Where(x => x.BarbershopId == selectedIndex), "Id", "Fullname"),
                ListPrices = new SelectList(_db.Prices, "Id", "Name")
            };
        }

        [Authorize]
        public ActionResult Record()
        {
            ViewBag.Message = "Record";
            
            return View(LoadRecordModel());
        }

        [HttpPost]
        [Authorize]
        public ActionResult Record(string barbershop, string employee, string price, Reception reception)
        {
            if (reception.DateTime.Hour < 8 || reception.DateTime.Hour > 19)
            {
                ViewBag.Error = "Парикмахерская работает с 8 до 20. Укажите другое время";
                return View(LoadRecordModel());
            }

            var employeeId = int.Parse(employee);
            var currentEmployee = _db.Employees.First(x => x.Id == employeeId);

            if (reception.DateTime.DayOfWeek == DayOfWeek.Sunday && currentEmployee.BarbershopId != 3)
            {
                ViewBag.Error = "Парикмахерская не работает в воскресенье. Пожалуйста, выберите другой день или 3 парикмахерскую.";
                return View(LoadRecordModel());
            }
            else if (reception.DateTime.DayOfWeek == DayOfWeek.Sunday && currentEmployee.BarbershopId == 3 && reception.DateTime.Hour > 17)
            {
                ViewBag.Error = "Парикмахерская работает с 8 до 18. Укажите другое время";
                return View(LoadRecordModel());
            }

            if (currentEmployee.BarbershopId == 3)
            {
                var receptionDay = reception.DateTime.Day;
                var currendDate = DateTime.Now;
                var nameTimeSheet = string.Join(string.Empty, currendDate.Month, "-", currendDate.Year);

                var timeSheet = _db.TimeSheets.First(x => x.Name.Contains(nameTimeSheet));

                var timeSheetFile = timeSheet.File;
                var tempFileName = Path.GetTempFileName();

                using (var stream = new FileStream(tempFileName, FileMode.Create))
                {
                    stream.Write(timeSheetFile, 0, timeSheetFile.Length);
                }

                using (var sr = new StreamReader(tempFileName, Encoding.GetEncoding(1251)))
                {
                    var line = sr.ReadLine();
                    while ((line = sr.ReadLine()) != null)
                    {
                        var strEmployeeId = line.Substring(0, line.IndexOf(';'));
                        if (int.Parse(strEmployeeId) != employeeId)
                        {
                            continue;
                        }

                        line = line.Replace(string.Join(string.Empty, strEmployeeId, ";"), string.Empty);

                        for (var i = 0; i < line.Split(';').Length; i++)
                        {
                            if (i + 1 != receptionDay)
                            {
                                continue;
                            }

                            var jobStatus = line.Split(';')[i];
                            if (jobStatus.Contains('о'))
                            {
                                ViewBag.Error = "Парикмахер в отпуске.";
                                return View(LoadRecordModel());
                            }
                            else if (jobStatus.Contains('у'))
                            {
                                ViewBag.Error = "Парикмахер уволен.";
                                return View(LoadRecordModel());
                            }
                            else if (jobStatus.Contains('б'))
                            {
                                ViewBag.Error = "Парикмахер на больничном.";
                                return View(LoadRecordModel());
                            }
                            else if (jobStatus.Contains('в'))
                            {
                                ViewBag.Error = "Парикмахер на выходном.";
                                return View(LoadRecordModel());
                            }
                        }
                    }
                }
            }

            var priceId = int.Parse(price);
            reception.PriceId = priceId;
            reception.Price = _db.Prices.First(x => x.Id == priceId);

            var success = true;
            var timeOfWorks = "К парихмахеру записаны:";
            List<Reception> listSelecteDayReception;
            using (var db = new NetworkHairdressingContext())
            {
                listSelecteDayReception = new List<Reception>(db.Receptions.Where(x =>
                    x.DateTime.Day == reception.DateTime.Day &&
                    x.DateTime.Month == reception.DateTime.Month &&
                    x.DateTime.Year == reception.DateTime.Year &&
                    x.EmployeeId == employeeId).OrderBy(x => x.DateTime).ToList());
            }

            foreach (var item in listSelecteDayReception)
            {
                var itemPriceDuration = _db.Prices.First(x => x.Id == item.PriceId).Duration;
                var receptionPriceDuration = _db.Prices.First(x => x.Id == reception.PriceId).Duration;

                var startTime = item.DateTime;
                var endTime = item.DateTime.AddMinutes(itemPriceDuration);

                if (reception.DateTime < endTime && startTime < reception.DateTime.AddMinutes(receptionPriceDuration))
                {
                    success = false;
                }

                timeOfWorks = string.Join(" ", timeOfWorks, "\n\tс ", startTime.ToShortTimeString(), " до ",
                    endTime.ToShortTimeString(), "\n");
            }

            if (!success)
            {
                ViewBag.Error = string.Join(" ", timeOfWorks, "Ваша услуга длится ", reception.Price.Duration, " минут.");
                return View(LoadRecordModel());
            }

            var dbReception = new Reception
            {
                DateTime = reception.DateTime,
                AspNetUsersId = User.Identity.GetUserId(),
                EmployeeId = currentEmployee.Id,
                PriceId = priceId
            };

            _db.Receptions.Add(dbReception);
            _db.SaveChanges();

            ViewBag.Success = "Вы успешно записались!";

            return RedirectToAction("Record");
        }

        [Authorize]
        public ActionResult GetItems(int id)
        {
            return PartialView(_db.Employees.Where(c => c.BarbershopId == id).ToList());
        }

        [Authorize]
        public ActionResult MyRecords()
        {
            List<Reception> receptions;
            using (var db = new NetworkHairdressingContext())
            {
                db.Employees.ToList();
                db.Prices.ToList();

                var userId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return HttpNotFound();
                }

                receptions = new List<Reception>(db.Receptions.Where(x => x.AspNetUsersId == userId).ToList());
            }

            return View(receptions);
        }

        [HttpPost]
        [Authorize]
        public ActionResult GetPrice(string price)
        {
            var priceId = int.Parse(price);
            var cost = _db.Prices.First(x => x.Id == priceId).Cost;
            return Json(new { Price = cost }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public ActionResult GetTimeWork(string id)
        {
            var employeeId = int.Parse(id);
            var isFirstShift = _db.Employees.First(x => x.Id == employeeId).IsFirstShift;
            return Json(new { IsFirstShift = isFirstShift }, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

        [Authorize]
        public ActionResult Unsubscribe(int id)
        {
            var reception = _db.Receptions.Find(id);

            if (reception.AspNetUsersId != User.Identity.GetUserId())
            {
                return HttpNotFound();
            }

            _db.Receptions.Remove(reception);
            _db.SaveChanges();

            return RedirectToAction("MyRecords");
        }
    }
}