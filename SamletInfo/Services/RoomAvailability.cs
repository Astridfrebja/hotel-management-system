using SamletInfo.Data;

namespace SamletInfo.Services
{
    public static class RoomAvailability
    {
        public static bool IsFree(HotelContext context, int roomId, DateTime from, DateTime to, int? exceptBookingId = null)
        {
            var fromDate = from.Date;
            var toDate = to.Date;

            return !context.Bookings.Any(b =>
                b.RoomId == roomId
                && (exceptBookingId == null || b.Id != exceptBookingId)
                && (b.Status == null || (b.Status != "CheckedOut" && b.Status != "Cancelled"))
                && b.CheckIn < toDate
                && fromDate < b.CheckOut);
        }
    }
}
