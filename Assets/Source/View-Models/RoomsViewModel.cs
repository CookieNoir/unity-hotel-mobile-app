using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Hotel.Models;
using Hotel.Networking;
using Hotel.UI;

namespace Hotel.ViewModels
{
    public class RoomsViewModel : MonoBehaviour, IViewModel
    {
        [SerializeField] private ClientHandler _clientHandler;
        [SerializeField] private SharedData _sharedData;
        [Space(10)]
        [SerializeField] private RoomCardHandler _roomCardHandler;
        [SerializeField] private NotificationHandler _notificationHandler;
        [SerializeField] private UnityEvent _OnRoomSelection;

        private static readonly List<string> _textFormats = new List<string>()
        {
            "Комната {0}",
            "макс. гостей: {0}",
            "за сутки: {0} руб.",
        };

        private static readonly Dictionary<string, string> _exceptions = new Dictionary<string, string>()
        {
            ["NO_CONNECTION"] = "Произошла ошибка при получении данных о комнатах",
        };

        public async void OnShow()
        {
            (ServerResponse, List<Room>) result = await _clientHandler.Client.GetRooms();
            if (result.Item1 != ServerResponse.Success)
            {
                _notificationHandler.Notify(_exceptions["NO_CONNECTION"]);
                return;
            }
            _roomCardHandler.CreateCards(result.Item2, _textFormats[0], _textFormats[1], _textFormats[2], SelectRoom);
        }

        public void OnHide()
        {
            _roomCardHandler.Clear();
        }

        public void SelectRoom(Room room)
        {
            _sharedData.Room = room;
            _OnRoomSelection.Invoke();
        }
    }
}