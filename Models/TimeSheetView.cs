using System.Collections.Generic;

namespace NetworkHairdressing.Models
{
    public class TimeSheetView
    {
        public TimeSheet TimeSheet { get; set; }
        public List<Dictionary<Employee, List<JobStatus>>> Text { get; set; }
        public int NumberOfDays { get; set; }

        public TimeSheetView(TimeSheet timeSheet, List<Dictionary<Employee, List<JobStatus>>> text, int numberOfDays)
        {
            TimeSheet = timeSheet;
            Text = new List<Dictionary<Employee, List<JobStatus>>>(text);
            NumberOfDays = numberOfDays;
        }
    }
}