using System.Data.Entity;

namespace NetworkHairdressing.Models
{
    public class NetworkHairdressingContext : DbContext
    {
        public DbSet<Barbershop> Barbershops { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeWork> EmployeeWorks { get; set; }

        public System.Data.Entity.DbSet<NetworkHairdressing.Models.TimeSheet> TimeSheets { get; set; }
    }
}