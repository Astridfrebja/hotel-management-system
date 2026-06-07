using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamletInfo.Models
{
    public class ServiceTask
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string Type { get; set; } = "";
        public string Status { get; set; } = "New";
        public string Note { get; set; } = "";

        public override string ToString()
        {
            return $"Rom {RoomId} - {Note}";
        }
    }
}
