namespace SamletInfo.Models
{
    public class Room
    {
        public int Id { get; set; }
        public int Beds { get; set; }
        public string Quality { get; set; } = "";
        public bool IsAvailable { get; set; }

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<ServiceTask> ServiceTasks { get; set; } = new List<ServiceTask>();
        public string RoomType => $"{Quality} - {Beds} senger";

    }
}
