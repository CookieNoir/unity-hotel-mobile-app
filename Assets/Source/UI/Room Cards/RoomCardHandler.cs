using System;
using System.Collections.Generic;
using UnityEngine;
using Hotel.Models;
using Hotel.Repositories;

namespace Hotel.UI
{
    public class RoomCardHandler : MonoBehaviour
    {
        [SerializeField] private GameObject _roomCardPrefab;
        [SerializeField] private Transform _parentObject;
        [SerializeField] private ImageRepository _imageRepository;
        private Dictionary<Room, RoomCard> _cards;
        private Func<Room, bool> _appliedFilter;

        private void Awake()
        {
            _cards = new Dictionary<Room, RoomCard>();
        }

        public void CreateCards(List<Room> rooms, string nameFormat, string bedsNumberFormat, string priceFormat, Action<Room> selectRoomAction)
        {
            foreach (Room room in rooms)
            {
                GameObject newCard = Instantiate(_roomCardPrefab, _parentObject);
                RoomCard roomCard = newCard.GetComponent<RoomCard>();
                roomCard.SetValues(room,
                    string.Format(nameFormat, room.RoomId),
                    string.Format(bedsNumberFormat, room.BedsNumber),
                    string.Format(priceFormat, room.Price),
                    _imageRepository,
                    room.ImagePath,
                    selectRoomAction);
                _cards.Add(room, roomCard);
            }
            ApplyFilter(_appliedFilter);
        }

        public void Clear()
        {
            foreach (Transform child in _parentObject)
            {
                Destroy(child.gameObject);
            }
            _cards.Clear();
        }

        public void ApplyFilter(Func<Room, bool> filter)
        {
            if (filter == null) return;
            foreach (KeyValuePair<Room, RoomCard> keyValuePair in _cards)
            {
                keyValuePair.Value.SetActive(filter(keyValuePair.Key));
            }
            _appliedFilter = filter;
        }
    }
}