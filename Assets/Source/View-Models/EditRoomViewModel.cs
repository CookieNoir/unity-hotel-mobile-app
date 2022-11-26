using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Hotel.Models;
using Hotel.Validation;
using Hotel.Networking;
using Hotel.Containers;
using Hotel.UI;

namespace Hotel.ViewModels
{
    public class EditRoomViewModel : MonoBehaviour, IViewModel
    {
        [SerializeField] private ClientHandler _clientHandler;
        [SerializeField] private SharedData _sharedData;
        [Space(10)]
        [SerializeField] private TMP_Text _roomName;
        [SerializeField] private TMP_InputField _bedsNumberInputField;
        [SerializeField] private TMP_InputField _priceInputField;
        [SerializeField] private TMP_InputField _imagePathInputField;
        [SerializeField] private TMP_Text _errorTextField;
        [Space(10)]
        [SerializeField] private NotificationHandler _notificationHandler;
        [SerializeField] private UnityEvent _OnSuccess;


        private static readonly List<string> _textFormats = new List<string>()
    {
        "Комната {0}",
    };

        private static readonly Dictionary<string, string> _messages = new Dictionary<string, string>()
        {
            ["EDIT_SUCCESS"] = "Данные о комнате успешно изменены",
            ["REMOVE_SUCCESS"] = "Данные о комнате успешно изменены",
            ["NO_CONNECTION"] = "Соединение с сервером не установлено. Попробуйте еще раз",
            ["NOT_VALID_BEDS_NUMBER"] = "Поле \"макс. гостей\" заполнено некорректно. Убедитесь, что ввели в этом поле натуральное число",
            ["NOT_VALID_PRICE"] = "Поле \"цена за сутки\" заполнено некорректно. Убедитесь, что ввели в этом поле натуральное число",
            ["NO_CHANGES"] = "Значения полей совпадают с начальными значениями комнаты",
        };

        public void OnShow()
        {
            _roomName.text = string.Format(_textFormats[0], _sharedData.Room.RoomId);
            _bedsNumberInputField.text = _sharedData.Room.BedsNumber.ToString();
            _priceInputField.text = _sharedData.Room.Price.ToString();
            _imagePathInputField.text = _sharedData.Room.ImagePath;
            _errorTextField.text = "";
        }

        public void OnHide()
        {

        }

        public async void Remove()
        {
            if (await _clientHandler.Client.RemoveRoom(_sharedData.Room.RoomId) != ServerResponse.Success)
            {
                _errorTextField.text = _messages["NO_CONNECTION"];
                return;
            }

            _notificationHandler.Notify(_messages["REMOVE_SUCCESS"]);
            _OnSuccess.Invoke();
        }

        public async void Edit()
        {
            int bedsNumber;
            try
            {
                bedsNumber = Convert.ToInt32(_bedsNumberInputField.text);
                if (!DataValidation.IsNaturalNumber(bedsNumber)) throw new Exception();
            }
            catch
            {
                _errorTextField.text = _messages["NOT_VALID_BEDS_NUMBER"];
                return;
            }

            int price;
            try
            {
                price = Convert.ToInt32(_priceInputField.text);
                if (!DataValidation.IsNaturalNumber(price)) throw new Exception();
            }
            catch
            {
                _errorTextField.text = _messages["NOT_VALID_PRICE"];
                return;
            }

            if (bedsNumber == _sharedData.Room.BedsNumber
                && price == _sharedData.Room.Price
                && _imagePathInputField.text == _sharedData.Room.ImagePath)
            {
                _errorTextField.text = _messages["NO_CHANGES"];
                return;
            }

            if (await _clientHandler.Client.EditRoom(new Room(_sharedData.Room.RoomId, bedsNumber, price, _imagePathInputField.text)) != ServerResponse.Success)
            {
                _errorTextField.text = _messages["NO_CONNECTION"];
                return;
            }

            _notificationHandler.Notify(_messages["EDIT_SUCCESS"]);
            _OnSuccess.Invoke();
        }
    }
}