namespace Hotel.Models 
{
    public class Role
    {
        public Role(bool canManageRooms, string roleName)
        {
            CanManageRooms = canManageRooms;
            RoleName = roleName;
        }

        public bool CanManageRooms { get; private set; }
        public string RoleName { get; private set; }
    }
}