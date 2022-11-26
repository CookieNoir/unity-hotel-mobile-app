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

        public void CreateCards(List<Room> rooms, string nameFormat, string bedsNumberFormat, string priceFormat, Action<int> action)
        {
            foreach (Room room in rooms)
            {
                GameObject newCard = Instantiate(_roomCardPrefab, _parentObject);
                RoomCard roomCard = newCard.GetComponent<RoomCard>();
                roomCard.Fill(room.RoomId,
                    string.Format(nameFormat, room.RoomId),
                    string.Format(bedsNumberFormat, room.BedsNumber),
                    string.Format(priceFormat, room.Price),
                    _imageRepository,
                    room.ImagePath,
                    action);
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