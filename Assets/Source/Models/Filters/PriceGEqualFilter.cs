namespace Hotel.Models
{
    public class PriceGEqualFilter : IRoomFilter
    {
        public int Value { get; set; }

        public bool Filter(Room room)
        {
            return room.Price >= Value;
        }
    }
}