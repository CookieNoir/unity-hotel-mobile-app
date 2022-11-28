using System.Collections.Generic;
using UnityEngine;
using Hotel.Networking;
using Hotel.Models;
using Hotel.UI;
using System.Threading.Tasks;

namespace Hotel.ViewModels
{
    public class BookingStatusViewModel : MonoBehaviour, IViewModel
    {
        [SerializeField] private ClientHandler _clientHandler;
        [Space(10)]
        [SerializeField] private BookingCardHandler _bookingCardHandler;
        [SerializeField] private NotificationHandler _notificationHandler;

        private static readonly List<string> _textFormats = new List<string>()
        {
            "Комната {0}",
            "{0} — {1}",
        };

        private static readonly Dictionary<string, string> _exceptions = new Dictionary<string, string>()
        {
            ["NO_CONNECTION_GET_BOOKINGS"] = "Произошла ошибка при получении данных о бронировании",
            ["NO_CONNECTION_CHANGE_STATUS"] = "Произошла ошибка при изменении статуса заказа",
        };

        public async void OnShow()
        {
            (ServerResponse, List<BookingData>) result = await _clientHandler.Client.GetBookings();
            if (result.Item1 != ServerResponse.Success)
            {
                _notificationHandler.Notify(_exceptions["NO_CONNECTION_GET_BOOKINGS"]);
                return;
            }
            List<BookingData> bookings = result.Item2;
            _bookingCardHandler.CreateCards(bookings, _textFormats[0], _textFormats[1], SetStage);
        }

        public void OnHide()
        {
            _bookingCardHandler.Clear();
        }

        public async Task<bool> SetStage(int bookingId, BookingStage stage)
        {
            if (await _clientHandler.Client.ChangeBookingStatus(bookingId, stage) != ServerResponse.Success)
            {
                _notificationHandler.Notify(_exceptions["NO_CONNECTION_CHANGE_STATUS"]);
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}