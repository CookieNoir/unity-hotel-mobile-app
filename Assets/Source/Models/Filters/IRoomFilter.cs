namespace Hotel.Models
{
    public interface IRoomFilter
    {
        bool Filter(Room room);

        int Value { get; set; }
    }
}