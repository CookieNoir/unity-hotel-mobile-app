using System.Collections.Generic;
using System.Threading.Tasks;

public interface IClient
{
    Task<ServerResponse> RegisterUser(string userName, string email, string phoneNumber, string password);
    Task<(ServerResponse, User)> GetUser(string email, string password);
    Task<(ServerResponse, List<Room>)> GetRooms();
    Task<ServerResponse> EditRoom(Room room);
    Task<ServerResponse> AddRoom(int bedsNumber, int price, string imagePath);
    Task<ServerResponse> RemoveRoom(int roomId);
}