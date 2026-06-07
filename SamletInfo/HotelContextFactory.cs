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

            optionsBuilder.UseSqlServer(
                "Server=<server>;Database=<database>;User ID=<username>;Password=<password>;"
            );

            return new HotelContext(optionsBuilder.Options);
        }
    }
}