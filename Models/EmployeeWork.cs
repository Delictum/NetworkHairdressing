namespace NetworkHairdressing.Models
{
    public class EmployeeWork
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public byte[] Image { get; set; }

        public Employee Employee { get; set; }
    }
}