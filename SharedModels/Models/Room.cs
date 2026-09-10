namespace SharedModels.Models
{
    public class Room
    {
        public int Id { get; set; }
        public int Beds { get; set; }
        public String Quality { get; set; }
        public bool IsAvailable { get; set; }

        public ICollection<Booking> Bookings { get; set; }
        public string RoomType => $"{Quality} - {Beds} senger";

    }
}
