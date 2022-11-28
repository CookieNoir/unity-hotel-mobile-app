using System;
using UnityEngine;
using TMPro;
using Hotel.Repositories;

namespace Hotel.UI
{
    public class RoomCard : MonoBehaviour
    {
        [SerializeField] private ImageFitter _imageFitter;
        [SerializeField] private TMP_Text _roomName;
        [SerializeField] private TMP_Text _bedsNumber;
        [SerializeField] private TMP_Text _price;
        private Action<int> _OnSelect;
        private int _roomId;

        public void SetValues(int roomId, string name, string bedsNumber, string price, ImageRepository imageRepository, string imagePath, Action<int> action)
        {
            _roomId = roomId;
            _roomName.text = name;
            _bedsNumber.text = bedsNumber;
            _price.text = price;
            _imageFitter.SetImage(imageRepository, imagePath);
            _OnSelect = action;
        }

        public void Select()
        {
            _OnSelect(_roomId);
        }
    }
}