using System;

namespace Hotel.Models
{
    public class UserBookingData
    {
        public UserBookingData(int roomId, int bedsNumber, int price, DateTime fromDate, DateTime toDate, BookingStage stage)
        {
            RoomId = roomId;
            BedsNumber = bedsNumber;
            Price = price;
            FromDate = fromDate;
            ToDate = toDate;
            Stage = stage;
        }

        public int RoomId { get; private set; }
        public int BedsNumber { get; private set; }
        public int Price { get; private set; }
        public DateTime FromDate { get; private set; }
        public DateTime ToDate { get; private set; }
        public BookingStage Stage { get; private set; }
    }
}