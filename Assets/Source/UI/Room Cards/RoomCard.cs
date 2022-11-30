using System;
using UnityEngine;
using TMPro;
using Hotel.Models;
using Hotel.Repositories;

namespace Hotel.UI
{
    public class RoomCard : UIToggler
    {
        [SerializeField] private ImageFitter _imageFitter;
        [SerializeField] private TMP_Text _roomName;
        [SerializeField] private TMP_Text _bedsNumber;
        [SerializeField] private TMP_Text _price;
        private Room _room;
        private Action<Room> _SelectRoomAction;

        public void SetValues(Room room, string name, string bedsNumber, string price, ImageRepository imageRepository, string imagePath, Action<Room> selectRoomAction)
        {
            _room = room;
            _roomName.text = name;
            _bedsNumber.text = bedsNumber;
            _price.text = price;
            _imageFitter.SetImage(imageRepository, imagePath);
            _SelectRoomAction = selectRoomAction;
        }

        public void Select()
        {
            _SelectRoomAction(_room);
        }
    }
}