
using Microsoft.EntityFrameworkCore;
using SamletInfo.Models; 

namespace SamletInfo.Data

{
    public class HotelContext : DbContext
    {
        public HotelContext(DbContextOptions<HotelContext> options) : base(options) { }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ServiceTask> ServiceTasks { get; set; }
        public DbSet<TaskTemplate> TaskTemplates { get; set; }


    }

}
