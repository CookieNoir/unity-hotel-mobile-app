using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Hotel.Models;
using Hotel.UI;
using Hotel.Repositories;

namespace Hotel.ViewModels
{
    public class RoomDetailsViewModel : MonoBehaviour, IViewModel
    {
        [SerializeField] private SharedData _sharedData;
        [Space(10)]
        [SerializeField] private ImageFitter _imageFitter;
        [SerializeField] private ImageRepository _imageRepository;
        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_Text _bedsNumber;
        [SerializeField] private TMP_Text _price;

        private static readonly List<string> _textFormats = new List<string>()
        {
            "������� {0}",
            "����. ������: {0}",
            "�� �����: {0} ���.",
        };

        public void OnShow()
        {
            _imageFitter.SetImage(_imageRepository, _sharedData.Room.ImagePath);
            _name.text = string.Format(_textFormats[0], _sharedData.Room.RoomId);
            _bedsNumber.text = string.Format(_textFormats[1], _sharedData.Room.BedsNumber);
            _price.text = string.Format(_textFormats[2], _sharedData.Room.Price);
        }

        public void OnHide()
        {

        }
    }
}