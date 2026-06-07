using System;
using System.ComponentModel.DataAnnotations;

namespace SamletInfo.Models

{
    public class BookingViewModel
    {
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Check-in Date")]
        public DateTime CheckInDate { get; set; }

        [Required]
        [Display(Name = "Room Type")]
        public string RoomType { get; set; }
    }
}

