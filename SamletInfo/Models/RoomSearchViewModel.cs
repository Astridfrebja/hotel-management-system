using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SamletInfo.Models

{
    public class RoomSearchViewModel
    {
        [Display(Name = "Number of Beds")]
        public int? NumberOfBeds { get; set; }

        [Display(Name = "Room Quality")]
        public string RoomQuality { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Check-in Date")]
        public DateTime? CheckInDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Check-out Date")]
        public DateTime? EndDate { get; set; }

        public List<Room> AvailableRooms { get; set; } = new List<Room>();
    }
}