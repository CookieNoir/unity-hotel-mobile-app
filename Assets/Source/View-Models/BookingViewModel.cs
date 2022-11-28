using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Hotel.Networking;
using Hotel.UI;
using TMPro;

namespace Hotel.ViewModels
{
    public class BookingViewModel : MonoBehaviour, IViewModel
    {
        [SerializeField] private ClientHandler _clientHandler;
        [SerializeField] private SharedData _sharedData;
        [Space(10)]
        [SerializeField] private TMP_Text _roomName;
        [SerializeField] private TMP_Text _bedsNumber;
        [SerializeField] private TMP_Text _price;
        [SerializeField] private CalendarHandler _fromDateCalendar;
        [SerializeField] private Slider _durationSlider;
        [SerializeField] private TMP_Text _bookedTextField;
        [SerializeField] private TMP_Text _errorTextField;
        [Space(10)]
        [SerializeField] private NotificationHandler _notificationHandler;
        [SerializeField] private UnityEvent _OnSuccess;
        private DateTime _fromDate;
        private int _duration;

        private static readonly List<string> _textFormats = new List<string>()
        {
            "Комната {0}",
            "макс. гостей: {0}",
            "за сутки: {0} руб.",
            "комната будет забронирована на следующие даты:\n{0} — {1}",
        };

        private static readonly Dictionary<string, string> _messages = new Dictionary<string, string>()
        {
            ["SUCCESS"] = "Комната успешно забронирована",
            ["NO_CONNECTION"] = "Соединение с сервером не установлено. Попробуйте еще раз",
        };

        public async void OnShow()
        {
            _roomName.text = string.Format(_textFormats[0], _sharedData.Room.RoomId);
            _bedsNumber.text = string.Format(_textFormats[1], _sharedData.Room.BedsNumber);
            _price.text = string.Format(_textFormats[2], _sharedData.Room.Price);

            DateTime serverDate = await _clientHandler.Client.GetCurrentDate();
            _fromDateCalendar.Fill(serverDate);
            OnSliderValueChanged(_durationSlider.value);

            _errorTextField.text = "";
        }

        public void OnHide()
        {

        }

        public void OnCalendarValueChanged(DateTime date)
        {
            _fromDate = date;
            _ShowBookedText();
        }

        public void OnSliderValueChanged(float value)
        {
            _duration = (int)value;
            _ShowBookedText();
        }

        private void _ShowBookedText()
        {
            DateTime toDate = _fromDate.AddDays(_duration);
            _bookedTextField.text = string.Format(_textFormats[3], _fromDate.ToString("dd.MM.yyyy"), toDate.ToString("dd.MM.yyyy"));
        }

        public async void Submit()
        {
            DateTime toDate = _fromDate.AddDays(_duration);

            if (await _clientHandler.Client.BookRoom(_sharedData.Room.RoomId, _sharedData.User.UserId, _fromDate, toDate) != ServerResponse.Success)
            {
                _errorTextField.text = _messages["NO_CONNECTION"];
                return;
            }

            _notificationHandler.Notify(_messages["SUCCESS"]);
            _OnSuccess.Invoke();
        }
    }
}