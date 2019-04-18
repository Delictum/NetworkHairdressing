namespace NetworkHairdressing.Models
{
    public class Price
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Cost { get; set; }
        public HaircutType HaircutType { get; set; }
        public int Duration { get; set; }
    }
}