using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hotel.Models;

namespace Hotel.Networking
{
    public interface IClient
    {
        Task<ServerResponse> RegisterUser(string userName, string email, string phoneNumber, string password);
        Task<(ServerResponse, User)> GetUser(string email, string password);
        Task<(ServerResponse, List<Room>)> GetRooms();
        Task<ServerResponse> EditRoom(Room room);
        Task<ServerResponse> AddRoom(int bedsNumber, int price, string imagePath);
        Task<ServerResponse> RemoveRoom(int roomId);
        Task<(ServerResponse, DateTime)> GetCurrentDate();
        Task<ServerResponse> BookRoom(int roomId, int userId, DateTime fromDate, DateTime toDate);
        Task<(ServerResponse, List<BookingData>)> GetBookings();
        Task<ServerResponse> ChangeBookingStatus(int bookingId, BookingStage stage);
        Task<(ServerResponse, HashSet<DateTime>)> GetBusyDays(int roomId, DateTime dateFrom, DateTime dateTo);
        Task<ServerResponse> EditRole(string email, Role role);
        Task<(ServerResponse, List<int>)> GetDecommissionedRooms();
        Task<ServerResponse> RestoreRoom(int roomId);
        Task<(ServerResponse, List<UserBookingData>)> GetUserBookings(int userId);
        Task<ServerResponse> EditUser(int userId, string userName, string phoneNumber, string password);
        Task<ServerResponse> EditPassword(int userId, string password, string newPassword);
    }
}