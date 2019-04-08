using System.Web.UI.WebControls;

namespace NetworkHairdressing.Models
{
    public class EmployeeWork
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public byte[] Image { get; set; }
    }
}