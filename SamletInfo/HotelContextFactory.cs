using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SamletInfo.Data;

namespace SamletInfo
{
    public class HotelContextFactory : IDesignTimeDbContextFactory<HotelContext>
    {
        public HotelContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<HotelContext>();
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                ?? "Server=localhost,1433;Database=HotelDb;User Id=sa;Password=HotelDev_123;TrustServerCertificate=True;Encrypt=True;MultipleActiveResultSets=True;";

            optionsBuilder.UseSqlServer(connectionString);

            return new HotelContext(optionsBuilder.Options);
        }
    }
}
