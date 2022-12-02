using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using UnityEngine;
using Hotel.Models;

namespace Hotel.Networking
{
    public class ClientServer : MonoBehaviour, IClient
    {
        private static NpgsqlDataSource _dataSource;
        private static string _connectionString = "Host=localhost;Username=postgres;Password=14999;Database=Hotel";
        private static Dictionary<string, string> _sqlCommands = new Dictionary<string, string>()
        {
            ["Test"] = "select version()",
            ["Register"] = "insert into \"User\" values (nextval('\"public\".\"User_user_id_seq\"'), 0, '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}')",
            ["Contains_User"] = "select * from \"User\" where \"email\" = '{0}'",
            ["Get_Role"] = "select \"can_manage_rooms\", \"can_manage_users\" from \"Role\" where \"role_id\" = {0}",
            ["Get_Rooms"] = "select \"room_id\", \"beds_number\", \"price\",\"image_path\" from \"Room\" where \"decommissioned\"=false",
            ["Edit_Room"] = "update \"Room\" set \"beds_number\"={1}, \"price\"={2}, \"image_path\"='{3}' where \"room_id\" = {0}",
            ["Add_Room"] = "insert into \"Room\" values (nextval('\"public\".\"Room_room_id_seq\"'), {0}, {1}, '{2}', false)",
            ["Remove_Room"] = "update \"Room\" set \"decommissioned\"=true where \"room_id\"={0}",
            ["Book_Room"] = "insert into \"Booking\" values (nextval('\"public\".\"Booking_booking_id_seq\"'), {0}, {1}, '{2}', '{3}');" +
                            "select currval('\"public\".\"Booking_booking_id_seq\"');",
            ["Add_Booking_Status"] = "insert into \"Booking_status\" values ({0}, 0)",
            ["Get_Bookings"] = "select \"Booking\".\"booking_id\", \"Booking\".\"room_id\", \"User\".\"email\", \"User\".\"phone_number\", \"Booking\".\"date_from\", \"Booking\".\"date_to\", \"Booking_status\".\"stage_id\", \"User\".\"key\", \"User\".\"IV\" " +
                            "from \"Booking_status\" join \"Booking\" on \"Booking\".\"booking_id\" = \"Booking_status\".\"booking_id\" " +
                            "join \"User\" on \"Booking\".\"user_id\" = \"User\".\"user_id\"",
            ["Change_Booking_Status"] = "update \"Booking_status\" set \"stage_id\" = {1} WHERE \"booking_id\" = {0}",
            ["Get_Busy_Days"] = "select \"Booking\".\"date_from\", \"Booking\".\"date_to\" from \"Booking\" " +
                                "where \"Booking\".\"room_id\" = {0} and \"Booking\".\"date_from\" <= '{2}' and \"Booking\".\"date_to\" >= '{1}'",
            ["Get_Busy_Count"] = "select count(*) from \"Booking\" where \"room_id\" = {0} and \"date_from\" <= '{2}' and \"date_to\" >= '{1}'",
            ["Get_Role_Id"] = "select \"role_id\" from \"Role\" where \"can_manage_rooms\"={0} and \"can_manage_users\"={1}",
            ["Edit_Role"] = "update \"User\" set \"role_id\" = {1} where \"email\"='{0}'",
            ["Restore_Room"] = "update \"Room\" set \"decommissioned\"=false where \"room_id\"={0}",
            ["Get_User_Bookings"] = "select \"Room\".\"room_id\", \"Room\".\"beds_number\", \"Room\".\"price\", \"Booking\".\"date_from\", \"Booking\".\"date_to\", \"Booking_status\".\"stage_id\" from \"Booking_status\" " +
                                    "join \"Booking\" on \"Booking_status\".\"booking_id\"=\"Booking\".\"booking_id\" " +
                                    "join \"Room\" on \"Room\".\"room_id\"=\"Booking\".\"room_id\" " +
                                    "where \"Booking\".\"user_id\"={0}",
            ["Get_Decommissioned_Rooms"] = "select \"room_id\" from \"Room\" where \"decommissioned\"=true",
            ["Get_Password"] = "select \"hash\", \"salt\" from \"User\" where \"user_id\"={0}",
            ["Edit_User"] = "update \"User\" set \"user_name\"='{1}', \"phone_number\"='{2}', \"key\"='{3}', \"IV\"='{4}' where \"user_id\"={0}",
            ["Edit_User_Password"] = "update \"User\" set \"hash\"='{1}', \"salt\"='{2}' where \"user_id\"={0}",
        };

        private async Task<NpgsqlConnection> _GetConnection()
        {
            if (_dataSource == null) _dataSource = NpgsqlDataSource.Create(_connectionString);
            return await _dataSource.OpenConnectionAsync();
        }

        private static bool _IsOpen(NpgsqlConnection connection)
        {
            return connection.State == System.Data.ConnectionState.Open;
        }

        public async Task<ServerResponse> RegisterUser(string userName, string email, string phoneNumber, string password)
        {
            using NpgsqlConnection connection = await _GetConnection();
            if (!_IsOpen(connection)) return ServerResponse.ConnectionError;

            string containsCommand = string.Format(_sqlCommands["Contains_User"], email);
            NpgsqlDataReader reader = await new NpgsqlCommand(containsCommand, connection).ExecuteReaderAsync();
            try
            {
                if (reader.HasRows) return ServerResponse.DataError;
            }
            catch
            {
                return ServerResponse.ConnectionError;
            }
            finally
            {
                await reader.CloseAsync();
            }

            NpgsqlTransaction transaction = null;
            try
            {
                transaction = await connection.BeginTransactionAsync();
                string salt = SecurityHelper.GenerateSalt();
                string hash = SecurityHelper.HashPassword(password, salt);
                string key, IV;
                string encryptedPhoneNumber = SecurityHelper.EncryptDataWithAes(phoneNumber, out key, out IV);
                string command = string.Format(_sqlCommands["Register"], userName, email, encryptedPhoneNumber, hash, salt, key, IV);
                await new NpgsqlCommand(command, connection).ExecuteNonQueryAsync();
                await transaction.CommitAsync();
                return ServerResponse.Success;
            }
            catch (Exception e)
            {
                Debug.Log(e.StackTrace);
                if (transaction != null) await transaction.RollbackAsync();
                return ServerResponse.ConnectionError;
            }
        }

        public async Task<(ServerResponse, User)> GetUser(string email, string password)
        {
            using NpgsqlConnection connection = await _GetConnection();
            if (!_IsOpen(connection)) return (ServerResponse.ConnectionError, null);

            string containsCommand = string.Format(_sqlCommands["Contains_User"], email);
            NpgsqlDataReader reader = await new NpgsqlCommand(containsCommand, connection).ExecuteReaderAsync();
            int userId = -1;
            int roleId = -1;
            string userName = null;
            string userEmail = null;
            string userPhoneNumber = null;
            string userHash = null;
            string userSalt = null;
            string key = null;
            string IV = null;
            try
            {
                if (await reader.ReadAsync())
                {
                    userId = (int)reader[0];
                    roleId = (int)reader[1];
                    userName = reader[2].ToString();
                    userEmail = reader[3].ToString();
                    userPhoneNumber = reader[4].ToString();
                    userHash = reader[5].ToString();
                    userSalt = reader[6].ToString();
                    key = reader[7].ToString();
                    IV = reader[8].ToString();
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.StackTrace);
                return (ServerResponse.ConnectionError, null);
            }
            finally
            {
                await reader.CloseAsync();
            }

            string hash = SecurityHelper.HashPassword(password, userSalt);
            if (hash == userHash)
            {

                string roleCommand = string.Format(_sqlCommands["Get_Role"], roleId);
                NpgsqlDataReader roleReader = await new NpgsqlCommand(roleCommand, connection).ExecuteReaderAsync();
                try
                {
                    if (await roleReader.ReadAsync())
                    {
                        bool value1 = (bool)roleReader[0];
                        bool value2 = (bool)roleReader[1];
                        Role role;
                        if (value2)
                        {
                            role = Role.Admin;
                        }
                        else if (value1)
                        {
                            role = Role.Receptionist;
                        }
                        else
                        {
                            role = Role.Customer;
                        }
                        string decryptedPhoneNumber = SecurityHelper.DecryptDataWithAes(userPhoneNumber, key, IV);
                        User user = new User(userId, role, userName, userEmail, decryptedPhoneNumber);
                        return (ServerResponse.Success, user);
                    }
                    return (ServerResponse.ConnectionError, null);
                }
                catch (Exception e)
                {
                    Debug.Log(e.StackTrace);
                    return (ServerResponse.ConnectionError, null);
                }
                finally
                {
                    await roleReader.CloseAsync();
                }
            }
            return (ServerResponse.DataError, null);
        }

        public async Task<(ServerResponse, List<Room>)> GetRooms()
        {
            using NpgsqlConnection connection = await _GetConnection();
            if (!_IsOpen(connection)) return (ServerResponse.ConnectionError, null);

            NpgsqlDataReader reader = await new NpgsqlCommand(_sqlCommands["Get_Rooms"], connection).ExecuteReaderAsync();
            try
            {
                List<Room> rooms = new List<Room>();
                while (await reader.ReadAsync())
                {
                    Room newRoom = new Room((int)reader[0], (int)reader[1], (int)reader[2], reader[3].ToString());
                    rooms.Add(newRoom);
                }
                return (ServerResponse.Success, rooms);
            }
            catch
            {
                return (ServerResponse.ConnectionError, null);
            }
            finally
            {
                await reader.CloseAsync();
            }
        }

        public async Task<ServerResponse> EditRoom(Room room)
        {
            using NpgsqlConnection connection = await _GetConnection();
            if (!_IsOpen(connection)) return ServerResponse.ConnectionError;

            NpgsqlTransaction transaction = null;
            try
            {
                transaction = await connection.BeginTransactionAsync();
                string command = string.Format(_sqlCommands["Edit_Room"], room.RoomId, room.BedsNumber, room.Price, room.ImagePath);
                await new NpgsqlCommand(command, connection).ExecuteNonQueryAsync();
                await transaction.CommitAsync();
                return ServerResponse.Success;
            }
            catch
            {
                if (transaction != null) await transaction.RollbackAsync();
                return ServerResponse.ConnectionError;
            }
        }

        public async Task<ServerResponse> AddRoom(int bedsNumber, int price, string imagePath)
        {
            using NpgsqlConnection connection = await _GetConnection();
            if (!_IsOpen(connection)) return ServerResponse.ConnectionError;

            NpgsqlTransaction transaction = null;
            try
            {
                transaction = await connection.BeginTransactionAsync();
                if (imagePath.Length == 0) imagePath = "placeholder";
                string command = string.Format(_sqlCommands["Add_Room"], bedsNumber, price, imagePath);
                await new NpgsqlCommand(command, connection).ExecuteNonQueryAsync();
                await transaction.CommitAsync();
                return ServerResponse.Success;
            }
            catch
            {
                if (transaction != null) await transaction.RollbackAsync();
                return ServerResponse.ConnectionError;
            }
        }

        public async Task<ServerResponse> RemoveRoom(int roomId)
        {
            using NpgsqlConnection connection = await _GetConnection();
            if (!_IsOpen(connection)) return ServerResponse.ConnectionError;

            NpgsqlTransaction transaction = null;
            try
            {
                transaction = await connection.BeginTransactionAsync();
                string command = string.Format(_sqlCommands["Remove_Room"], roomId);
                await new NpgsqlCommand(command, connection).ExecuteNonQueryAsync();
                await transaction.CommitAsync();
                return ServerResponse.Success;
            }
            catch
            {
                if (transaction != null) await transaction.RollbackAsync();
                return ServerResponse.ConnectionError;
            }
        }

        public Task<(ServerResponse, DateTime)> GetCurrentDate()
        {
            return Task.FromResult((ServerResponse.Success, DateTime.Today));
        }

        public async Task<ServerResponse> BookRoom(int roomId, int userId, DateTime fromDate, DateTime toDate)
        {
            using NpgsqlConnection connection = await _GetConnection();
            if (!_IsOpen(connection)) return ServerResponse.ConnectionError;

            NpgsqlTransaction transaction = null;
            try
            {
                transaction = await connection.BeginTransactionAsync();
                string countCommand = string.Format(_sqlCommands["Get_Busy_Count"], roomId, fromDate.ToString("yyyy-MM-dd"), toDate.ToString("yyyy-MM-dd"));
                NpgsqlDataReader countReader = await new NpgsqlCommand(countCommand, connection).ExecuteReaderAsync();
                try
                {
                    if (await countReader.ReadAsync())
                    {
                        if ((long)countReader[0] > 0) throw new ArgumentOutOfRangeException();
                    }
                }
                finally
                {
                    await countReader.CloseAsync();
                }

                string command = string.Format(_sqlCommands["Book_Room"], roomId, userId, fromDate.ToString("yyyy-MM-dd"), toDate.ToString("yyyy-MM-dd"));
                NpgsqlDataReader reader = await new NpgsqlCommand(command, connection).ExecuteReaderAsync();
                object bookingId = null;
                try
                {
                    if (await reader.ReadAsync())
                    {
                        bookingId = reader[0];
                    }
                }
                finally
                {
                    await reader.CloseAsync();
                }

                string statusCommand = string.Format(_sqlCommands["Add_Booking_Status"], bookingId);
                await new NpgsqlCommand(statusCommand, connection).ExecuteNonQueryAsync();
                await transaction.CommitAsync();
                return ServerResponse.Success;
            }
            catch (Exception e)
            {
                if (transaction != null) await transaction.RollbackAsync();
                if (e.GetType() == typeof(ArgumentOutOfRangeException)) return ServerResponse.DataError;
                else return ServerResponse.ConnectionError;
            }
        }

        public async Task<(ServerResponse, List<BookingData>)> GetBookings()
        {
            using NpgsqlConnection connection = await _GetConnection();
            if (!_IsOpen(connection)) return (ServerResponse.ConnectionError, null);

            NpgsqlDataReader reader = await new NpgsqlCommand(_sqlCommands["Get_Bookings"], connection).ExecuteReaderAsync();
            try
            {
                List<BookingData> bookings = new List<BookingData>();
                while (await reader.ReadAsync())
                {
                    string decryptedPhoneNumber = SecurityHelper.DecryptDataWithAes(reader[3].ToString(), reader[7].ToString(), reader[8].ToString());
                    BookingData booking = new BookingData((int)reader[0], (int)reader[1],
                        reader[2].ToString(), decryptedPhoneNumber,
                        (DateTime)reader[4], (DateTime)reader[5],
                        (BookingStage)((int)reader[6]));
                    bookings.Add(booking);
                }
                return (ServerResponse.Success, bookings);
            }
            catch (Exception e)
            {
                Debug.Log(e.StackTrace);
                return (ServerResponse.ConnectionError, null);
            }
            finally
            {
                await reader.CloseAsync();
            }
        }

        public async Task<ServerResponse> ChangeBookingStatus(int bookingId, BookingStage stage)
        {
            using NpgsqlConnection connection = await _GetConnection();
            if (!_IsOpen(connection)) return ServerResponse.ConnectionError;

            NpgsqlTransaction transaction = null;
            try
            {
                transaction = await connection.BeginTransactionAsync();
                string command = string.Format(_sqlCommands["Change_Booking_Status"], bookingId, (int)stage);
                await new NpgsqlCommand(command, connection).ExecuteNonQueryAsync();
                await transaction.CommitAsync();
                return ServerResponse.Success;
            }
            catch
            {
                if (transaction != null) await transaction.RollbackAsync();
                return ServerResponse.ConnectionError;
            }
        }

        public async Task<(ServerResponse, HashSet<DateTime>)> GetBusyDays(int roomId, DateTime dateFrom, DateTime dateTo)
        {
            using NpgsqlConnection connection = await _GetConnection();
            if (!_IsOpen(connection)) return (ServerResponse.ConnectionError, null);

            string command = string.Format(_sqlCommands["Get_Busy_Days"], roomId, dateFrom.ToString("yyyy-MM-dd"), dateTo.ToString("yyyy-MM-dd"));
            NpgsqlDataReader reader = await new NpgsqlCommand(command, connection).ExecuteReaderAsync();
            try
            {
                HashSet<DateTime> busyDays = new HashSet<DateTime>();
                while (await reader.ReadAsync())
                {
                    DateTime fromBusy = (DateTime)reader[0];
                    DateTime toBusy = (DateTime)reader[1];
                    TimeSpan difference = toBusy - fromBusy;
                    for (int i = 0; i <= difference.Days; ++i)
                    {
                        busyDays.Add(fromBusy.AddDays(i));
                    }
                }
                return (ServerResponse.Success, busyDays);
            }
            catch
            {
                return (ServerResponse.ConnectionError, null);
            }
            finally
            {
                await reader.CloseAsync();
            }
        }

        public async Task<ServerResponse> EditRole(string email, Role role)
        {
            using NpgsqlConnection connection = await _GetConnection();
            if (!_IsOpen(connection)) return ServerResponse.ConnectionError;

            NpgsqlTransaction transaction = null;
            try
            {
                transaction = await connection.BeginTransactionAsync();
                string containsCommand = string.Format(_sqlCommands["Contains_User"], email);
                NpgsqlDataReader containsReader = await new NpgsqlCommand(containsCommand, connection).ExecuteReaderAsync();
                try
                {
                    if (!containsReader.HasRows) throw new ArgumentException();
                }
                finally
                {
                    await containsReader.CloseAsync();
                }

                string value0 = "", value1 = "";
                switch (role)
                {
                    case Role.Customer:
                        {
                            value0 = "false";
                            value1 = "false";
                            break;
                        }
                    case Role.Receptionist:
                        {
                            value0 = "true";
                            value1 = "false";
                            break;
                        }
                    case Role.Admin:
                        {
                            value0 = "true";
                            value1 = "true";
                            break;
                        }
                }
                string getRoleIdCommand = string.Format(_sqlCommands["Get_Role_Id"], value0, value1);
                NpgsqlDataReader getRoleReader = await new NpgsqlCommand(getRoleIdCommand, connection).ExecuteReaderAsync();
                int roleId = -1;
                try
                {
                    if (await getRoleReader.ReadAsync()) roleId = (int)getRoleReader[0];
                }
                finally
                {
                    await getRoleReader.CloseAsync();
                }

                string editRoleCommand = string.Format(_sqlCommands["Edit_Role"], email, roleId);
                await new NpgsqlCommand(editRoleCommand, connection).ExecuteNonQueryAsync();
                await transaction.CommitAsync();
                return ServerResponse.Success;
            }
            catch (Exception e)
            {
                if (transaction != null) await transaction.RollbackAsync();
                if (e.GetType() == typeof(ArgumentException)) return ServerResponse.DataError;
                return ServerResponse.ConnectionError;
            }
        }

        public async Task<ServerResponse> RestoreRoom(int roomId)
        {
            using NpgsqlConnection connection = await _GetConnection();
            if (!_IsOpen(connection)) return ServerResponse.ConnectionError;

            NpgsqlTransaction transaction = null;
            try
            {
                transaction = await connection.BeginTransactionAsync();
                string command = string.Format(_sqlCommands["Restore_Room"], roomId);
                await new NpgsqlCommand(command, connection).ExecuteNonQueryAsync();
                await transaction.CommitAsync();
                return ServerResponse.Success;
            }
            catch
            {
                if (transaction != null) await transaction.RollbackAsync();
                return ServerResponse.ConnectionError;
            }
        }

        public async Task<(ServerResponse, List<UserBookingData>)> GetUserBookings(int userId)
        {
            using NpgsqlConnection connection = await _GetConnection();
            if (!_IsOpen(connection)) return (ServerResponse.ConnectionError, null);

            string command = string.Format(_sqlCommands["Get_User_Bookings"], userId);
            NpgsqlDataReader reader = await new NpgsqlCommand(command, connection).ExecuteReaderAsync();
            try
            {
                List<UserBookingData> bookings = new List<UserBookingData>();
                while (await reader.ReadAsync())
                {
                    bookings.Add(new UserBookingData((int)reader[0], (int)reader[1], (int)reader[2], (DateTime)reader[3], (DateTime)reader[4], (BookingStage)reader[5]));
                }
                return (ServerResponse.Success, bookings);
            }
            catch
            {
                return (ServerResponse.ConnectionError, null);
            }
            finally
            {
                await reader.CloseAsync();
            }
        }

        public async Task<(ServerResponse, List<int>)> GetDecommissionedRooms()
        {
            using NpgsqlConnection connection = await _GetConnection();
            if (!_IsOpen(connection)) return (ServerResponse.ConnectionError, null);

            NpgsqlDataReader reader = await new NpgsqlCommand(_sqlCommands["Get_Decommissioned_Rooms"], connection).ExecuteReaderAsync();
            try
            {
                List<int> roomIds = new List<int>();
                while (await reader.ReadAsync())
                {
                    roomIds.Add((int)reader[0]);
                }
                return (ServerResponse.Success, roomIds);
            }
            catch
            {
                return (ServerResponse.ConnectionError, null);
            }
            finally
            {
                await reader.CloseAsync();
            }
        }

        public async Task<ServerResponse> EditUser(int userId, string userName, string phoneNumber, string password)
        {
            using NpgsqlConnection connection = await _GetConnection();
            if (!_IsOpen(connection)) return ServerResponse.ConnectionError;

            NpgsqlTransaction transaction = null;
            try
            {
                transaction = await connection.BeginTransactionAsync();
                string getPasswordCommand = string.Format(_sqlCommands["Get_Password"], userId);
                NpgsqlDataReader passwordReader = await new NpgsqlCommand(getPasswordCommand, connection).ExecuteReaderAsync();
                string userHash = null;
                string userSalt = null;
                try
                {
                    if (await passwordReader.ReadAsync())
                    {
                        userHash = passwordReader[0].ToString();
                        userSalt = passwordReader[1].ToString();
                    }
                }
                finally
                {
                    await passwordReader.CloseAsync();
                }

                string hash = SecurityHelper.HashPassword(password, userSalt);
                if (hash != userHash)
                {
                    throw new KeyNotFoundException();
                }

                string key, IV;
                string encryptedPhoneNumber = SecurityHelper.EncryptDataWithAes(phoneNumber, out key, out IV);
                string editUserCommand = string.Format(_sqlCommands["Edit_User"], userId, userName, encryptedPhoneNumber, key, IV);
                await new NpgsqlCommand(editUserCommand, connection).ExecuteNonQueryAsync();
                await transaction.CommitAsync();
                return ServerResponse.Success;
            }
            catch (Exception e)
            {
                if (transaction != null) await transaction.RollbackAsync();
                if (e.GetType() == typeof(KeyNotFoundException)) return ServerResponse.DataError;
                return ServerResponse.ConnectionError;
            }
        }

        public async Task<ServerResponse> EditPassword(int userId, string password, string newPassword)
        {
            using NpgsqlConnection connection = await _GetConnection();
            if (!_IsOpen(connection)) return ServerResponse.ConnectionError;

            NpgsqlTransaction transaction = null;
            try
            {
                transaction = await connection.BeginTransactionAsync();
                string getPasswordCommand = string.Format(_sqlCommands["Get_Password"], userId);
                NpgsqlDataReader passwordReader = await new NpgsqlCommand(getPasswordCommand, connection).ExecuteReaderAsync();
                string userHash = null;
                string userSalt = null;
                try
                {
                    if (await passwordReader.ReadAsync())
                    {
                        userHash = passwordReader[0].ToString();
                        userSalt = passwordReader[1].ToString();
                    }
                }
                finally
                {
                    await passwordReader.CloseAsync();
                }

                string hash = SecurityHelper.HashPassword(password, userSalt);
                if (hash != userHash)
                {
                    throw new KeyNotFoundException();
                }

                string newSalt = SecurityHelper.GenerateSalt();
                string newHash = SecurityHelper.HashPassword(newPassword, newSalt);
                string editUserCommand = string.Format(_sqlCommands["Edit_User_Password"], userId, newHash, newSalt);
                await new NpgsqlCommand(editUserCommand, connection).ExecuteNonQueryAsync();
                await transaction.CommitAsync();
                return ServerResponse.Success;
            }
            catch (Exception e)
            {
                if (transaction != null) await transaction.RollbackAsync();
                if (e.GetType() == typeof(KeyNotFoundException)) return ServerResponse.DataError;
                return ServerResponse.ConnectionError;
            }
        }
    }
}