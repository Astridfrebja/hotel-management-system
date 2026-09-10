using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SamletInfo.Models

{
    public class RoomSearchViewModel
    {
        [Display(Name = "Antall senger")]
        public int? NumberOfBeds { get; set; }

        [Display(Name = "Kvalitet")]
        public string? RoomQuality { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Innsjekk")]
        public DateTime? CheckInDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Utsjekk")]
        public DateTime? EndDate { get; set; }

        public List<Room> AvailableRooms { get; set; } = new List<Room>();
    }
}