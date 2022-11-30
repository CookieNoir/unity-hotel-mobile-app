using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Hotel.Networking;
using Hotel.UI;

namespace Hotel.ViewModels
{
    public class RestoreRoomsViewModel : MonoBehaviour, IViewModel
    {
        [SerializeField] private ClientHandler _clientHandler;
        [Space(10)]
        [SerializeField] private DecommissionedRoomHandler _decommissionedRoomHandler;
        [Space(10)]
        [SerializeField] private NotificationHandler _notificationHandler;

        private static readonly List<string> _textFormats = new List<string>()
        {
            "Комната {0}",
        };
        private static readonly Dictionary<string, string> _messages = new Dictionary<string, string>()
        {
            ["SUCCESS"] = "Комната успешно восстановлена",
            ["NO_CONNECTION"] = "Соединение с сервером не установлено. Попробуйте еще раз",
        };

        public async void OnShow()
        {
            (ServerResponse, List<int>) result = await _clientHandler.Client.GetDecommissionedRooms();
            if (result.Item1 != ServerResponse.Success)
            {
                _notificationHandler.Notify(_messages["NO_CONNECTION"]);
                return;
            }

            _decommissionedRoomHandler.CreateCards(_textFormats[0], result.Item2, Restore);
        }

        public void OnHide()
        {
            _decommissionedRoomHandler.Clear();
        }

        public async Task<bool> Restore(int roomId)
        {
            if (await _clientHandler.Client.RestoreRoom(roomId) != ServerResponse.Success)
            {
                _notificationHandler.Notify(_messages["NO_CONNECTION"]);
                return false;
            }
            _notificationHandler.Notify(_messages["SUCCESS"]);
            return true;
        }
    }
}