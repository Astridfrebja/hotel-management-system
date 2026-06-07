using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Models
{
    public class ServiceTask
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string Type { get; set; } // "Cleaning", "Maintenance", "Service"
        public string Status { get; set; } // "New", "InProgress", "Done"
        public string Note { get; set; }
    }
}
