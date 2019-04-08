using System.Web.UI.WebControls;

namespace NetworkHairdressing.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Fullname { get; set; }
        public int BarbershopId { get; set; }
        public byte[] Image { get; set; }
    }
}