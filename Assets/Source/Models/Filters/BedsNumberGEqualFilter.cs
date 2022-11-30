namespace Hotel.Models
{
    public class BedsNumberGEqualFilter : IRoomFilter
    {
        public int Value { get; set; }

        public bool Filter(Room room)
        {
            return room.BedsNumber >= Value;
        }
    }
}