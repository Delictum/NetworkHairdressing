using System.Data.Entity;

namespace NetworkHairdressing.Models
{
    public class NetworkHairdressingContext : DbContext
    {
        public DbSet<Barbershop> Barbershops { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeWork> EmployeeWorks { get; set; }

        public DbSet<TimeSheet> TimeSheets { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<Reception> Receptions { get; set; }
    }
}