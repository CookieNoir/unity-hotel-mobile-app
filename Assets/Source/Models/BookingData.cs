using System;

namespace Hotel.Models
{
    public class BookingData
    {
        public BookingData(int bookingId, int roomId, string customerEmail, string customerPhoneNumber, DateTime dateFrom, DateTime dateTo, BookingStage stage)
        {
            BookingId = bookingId;
            RoomId = roomId;
            CustomerEmail = customerEmail;
            CustomerPhoneNumber = customerPhoneNumber;
            DateFrom = dateFrom;
            DateTo = dateTo;
            BookingStage = stage;
        }

        public int BookingId { get; private set; }
        public int RoomId { get; private set; }
        public string CustomerEmail { get; private set; }
        public string CustomerPhoneNumber { get; private set; }
        public DateTime DateFrom { get; private set; }
        public DateTime DateTo { get; private set; }
        public BookingStage BookingStage { get; private set; }
    }
}