using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Hotel.Models;
using Hotel.Networking;
using Hotel.Containers;
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
        [Space(10)]
        [SerializeField] private GameObject _addRoomButton;
        [SerializeField] private GameObject _bookingStatusButton;
        private Dictionary<int, Room> _rooms = new Dictionary<int, Room>();

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
            List<Room> rooms = result.Item2;
            rooms.Sort();
            foreach (Room room in rooms)
            {
                _rooms.Add(room.RoomId, room);
            }
            _roomCardHandler.CreateCards(rooms, _textFormats[0], _textFormats[1], _textFormats[2], SelectRoom);
            _SetSpecialButtonsActive(_sharedData.User.Role.CanManageRooms);
        }

        private void _SetSpecialButtonsActive(bool value)
        {
            _addRoomButton.SetActive(value);
            _bookingStatusButton.SetActive(value);
        }

        public void OnHide()
        {
            _roomCardHandler.Clear();
            _rooms.Clear();
            _SetSpecialButtonsActive(false);
        }

        public void SelectRoom(int roomId)
        {
            _sharedData.Room = _rooms[roomId];
            _OnRoomSelection.Invoke();
        }
    }
}