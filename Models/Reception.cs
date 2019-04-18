using System;

namespace NetworkHairdressing.Models
{
    public class Reception
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int PriceId { get; set; }
        public string AspNetUsersId { get; set; }
        public DateTime DateTime { get; set; } 

        public Employee Employee { get; set; }
        public Price Price { get; set; }
    }
}