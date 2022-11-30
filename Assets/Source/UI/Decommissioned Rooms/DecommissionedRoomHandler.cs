using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Hotel.UI
{
    public class DecommissionedRoomHandler : MonoBehaviour
    {
        [SerializeField] private GameObject _roomCardPrefab;
        [SerializeField] private Transform _parentObject;

        public void CreateCards(string roomNameFormat, List<int> roomIds, Func<int, Task<bool>> restoreFunc)
        {
            foreach (int roomId in roomIds)
            {
                GameObject newCard = Instantiate(_roomCardPrefab, _parentObject);
                DecommissionedRoomCard roomCard = newCard.GetComponent<DecommissionedRoomCard>();
                roomCard.SetValues(string.Format(roomNameFormat, roomId), roomId, restoreFunc);
            }
        }

        public void Clear()
        {
            foreach (Transform child in _parentObject)
            {
                Destroy(child.gameObject);
            }
        }
    }
}