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
            ["Register"] = "insert into \"User\" values (nextval('\"public\".\"User_user_id_seq\"'), 0, '{0}', '{1}', '{2}', '{3}', '{4}')",
            ["Contains_User"] = "select * from \"User\" where \"email\" = '{0}'",
            ["Get_Role"] = "select * from \"Role\" where \"role_id\" = {0}",
            ["Get_Rooms"] = "select * from \"Room\"",
            ["Edit_Room"] = "update \"Room\" set \"beds_number\"={1}, \"price\"={2}, \"image_path\"='{3}' where \"room_id\" = {0}",
            ["Add_Room"] = "insert into \"Room\" values (nextval('\"public\".\"Room_room_id_seq\"'), {0}, {1}, '{2}')",
            ["Remove_Room"] = "delete from \"Room\" where \"room_id\"={0}",
            ["Book_Room"] = "insert into \"Booking\" values (nextval('\"public\".\"Booking_booking_id_seq\"'), {0}, {1}, '{2}', '{3}');" +
                            "select currval('\"public\".\"Booking_booking_id_seq\"');",
            ["Add_Booking_Status"] = "insert into \"Booking_status\" values ({0}, 0)",
            ["Get_Bookings"] = "select \"Booking\".\"booking_id\", \"Booking\".\"room_id\", \"User\".\"email\", \"User\".\"phone_number\", \"Booking\".\"date_from\", \"Booking\".\"date_to\", \"Booking_status\".\"stage_id\" " +
                            "from \"Booking_status\" join \"Booking\" on \"Booking\".\"booking_id\" = \"Booking_status\".\"booking_id\" " +
                            "join \"User\" on \"Booking\".\"user_id\" = \"User\".\"user_id\"",
            ["Change_Booking_Status"] = "update \"Booking_status\" set \"stage_id\" = {1} WHERE \"booking_id\" = {0}",
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
                string command = string.Format(_sqlCommands["Register"], userName, email, phoneNumber, hash, salt);
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
                }
            }
            catch
            {
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
                        Role role = new Role((bool)roleReader[1], roleReader[2].ToString());
                        User user = new User(userId, role, userName, userEmail, userPhoneNumber, userHash, userSalt);
                        return (ServerResponse.Success, user);
                    }
                    return (ServerResponse.ConnectionError, null);
                }
                catch
                {
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

        public Task<DateTime> GetCurrentDate()
        {
            return Task.FromResult(DateTime.Today);
        }

        public async Task<ServerResponse> BookRoom(int roomId, int userId, DateTime fromDate, DateTime toDate)
        {
            using NpgsqlConnection connection = await _GetConnection();
            if (!_IsOpen(connection)) return ServerResponse.ConnectionError;

            NpgsqlTransaction transaction = null;
            try
            {
                transaction = await connection.BeginTransactionAsync();
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
            catch
            {
                if (transaction != null) await transaction.RollbackAsync();
                return ServerResponse.ConnectionError;
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
                    BookingData booking = new BookingData((int)reader[0], (int)reader[1], 
                        reader[2].ToString(), reader[3].ToString(), 
                        (DateTime)reader[4], (DateTime)reader[5], 
                        (BookingStage)((int)reader[6]));
                    bookings.Add(booking);
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
    }
}