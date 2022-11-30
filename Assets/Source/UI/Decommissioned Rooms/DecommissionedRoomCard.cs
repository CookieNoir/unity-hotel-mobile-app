using System;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

namespace Hotel.UI
{
    public class DecommissionedRoomCard : MonoBehaviour
    {
        [SerializeField] private TMP_Text _roomName;
        private int _roomId;
        private Func<int, Task<bool>> _RestoreFunc;

        public void SetValues(string roomName, int roomId, Func<int, Task<bool>> restoreFunc)
        {
            _roomName.text = roomName;
            _roomId = roomId;
            _RestoreFunc = restoreFunc;
        }

        public async void Restore()
        {
            if (await _RestoreFunc(_roomId))
            {
                Destroy(gameObject);
            }
        }
    }
}