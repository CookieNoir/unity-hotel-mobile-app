namespace Hotel.Models
{
    public class PriceLEqualFilter : IRoomFilter
    {
        public int Value { get; set; }

        public bool Filter(Room room)
        {
            return room.Price <= Value;
        }
    }
}