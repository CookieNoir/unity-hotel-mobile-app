namespace Hotel.Models
{
    public class User
    {
        public User(int userId, Role role, string name, string email, string phoneNumber, string hash, string salt)
        {
            UserId = userId;
            Role = role;
            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
            Hash = hash;
            Salt = salt;
        }

        public int UserId { get; private set; }
        public Role Role { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string PhoneNumber { get; private set; }
        public string Hash { get; private set; }
        public string Salt { get; private set; }
    }
}