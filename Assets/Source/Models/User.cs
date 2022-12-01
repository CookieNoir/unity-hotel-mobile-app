namespace Hotel.Models
{
    public class User
    {
        public User(int userId, Role role, string name, string email, string phoneNumber)
        {
            UserId = userId;
            Role = role;
            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
        }

        public int UserId { get; private set; }
        public Role Role { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string PhoneNumber { get; private set; }

        public void EditNameAndPhoneNumber(string newName, string newPhoneNumber)
        {
            Name = newName;
            PhoneNumber = newPhoneNumber;
        }
    }
}