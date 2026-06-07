using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamletInfo.Models
{
    public class TaskTemplate
    {
        public int Id { get; set; }
        public string Note { get; set; }  // Beskrivelse
        public string Type { get; set; }  // "Cleaner", "Maintenance", "Service"
    }

}
