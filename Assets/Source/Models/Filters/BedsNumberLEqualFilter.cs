namespace Hotel.Models
{
    public class BedsNumberLEqualFilter : IRoomFilter
    {
        public int Value { get; set; }

        public bool Filter(Room room)
        {
            return room.BedsNumber <= Value;
        }
    }
}