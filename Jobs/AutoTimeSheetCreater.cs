using NetworkHairdressing.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkHairdressing.Jobs
{
    public class AutoTimeSheetCreater : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            if (DateTime.Now.Day == 1)
            {
                using (var db = new NetworkHairdressingContext())
                {

                    var currendDate = DateTime.Now.AddMonths(1);
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

                    await db.SaveChangesAsync();
                }
            }
        }
    }
}