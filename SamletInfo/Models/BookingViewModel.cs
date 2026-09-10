using System;
using System.ComponentModel.DataAnnotations;

namespace SamletInfo.Models

{
    public class BookingViewModel
    {
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Innsjekk")]
        public DateTime CheckInDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Utsjekk")]
        public DateTime CheckOutDate { get; set; }

        [Display(Name = "Romtype")]
        public string? RoomType { get; set; }
    }
}

