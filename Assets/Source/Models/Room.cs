using System;

public class Room : IComparable<Room>
{
    public Room(int roomId, int bedsNumber, int price, string imagePath)
    {
        RoomId = roomId;
        BedsNumber = bedsNumber;
        Price = price;
        ImagePath = imagePath;
    }

    public int RoomId { get; private set; }
    public int BedsNumber { get; private set; }
    public int Price { get; private set; }
    public string ImagePath { get; private set; }

    public int CompareTo(Room other)
    {
        return RoomId.CompareTo(other.RoomId);
    }
}