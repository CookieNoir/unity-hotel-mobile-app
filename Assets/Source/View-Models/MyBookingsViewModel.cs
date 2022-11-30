using System.Collections.Generic;
using UnityEngine;
using Hotel.Networking;
using Hotel.Models;
using Hotel.UI;

namespace Hotel.ViewModels
{
    public class MyBookingsViewModel : MonoBehaviour, IViewModel
    {
        [SerializeField] private ClientHandler _clientHandler;
        [SerializeField] private SharedData _sharedData;
        [Space(10)]
        [SerializeField] private UserBookingCardHandler _userBookingCardHandler;
        [Space(10)]
        [SerializeField] private NotificationHandler _notificationHandler;

        private static readonly List<string> _textFormats = new List<string>()
        {
            "Комната {0}",
            "макс. гостей: {0}",
            "за сутки: {0} руб.",
            "{0} — {1}",
        };

        private static readonly Dictionary<string, string> _messages = new Dictionary<string, string>()
        {
            ["NO_CONNECTION"] = "Соединение с сервером не установлено. Попробуйте еще раз",
        };

        public async void OnShow()
        {
            (ServerResponse, List<UserBookingData>) result = await _clientHandler.Client.GetUserBookings(_sharedData.User.UserId);
            if (result.Item1 != ServerResponse.Success)
            {
                _notificationHandler.Notify(_messages["NO_CONNECTION"]);
                return;
            }

            _userBookingCardHandler.CreateCards(result.Item2, _textFormats[0], _textFormats[1], _textFormats[2], _textFormats[3]);
        }

        public void OnHide()
        {
            _userBookingCardHandler.Clear();
        }
    }
}