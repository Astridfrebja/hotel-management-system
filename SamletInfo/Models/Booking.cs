using System.ComponentModel.DataAnnotations;
using SamletInfo.Data;
using SamletInfo.Models; 

namespace SamletInfo.Models

{
    public class Booking
    {
        public int Id { get; set; }
        public String CustomerEmail { get; set; }

        [DataType(DataType.Date)]
        public DateTime CheckIn { get; set; }
        [DataType(DataType.Date)]
        public DateTime CheckOut { get; set; }

        public int RoomId { get; set; }
        public Room? Room { get; set; }
    }
}

