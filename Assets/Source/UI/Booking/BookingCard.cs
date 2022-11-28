using System;
using UnityEngine;
using TMPro;
using Hotel.Models;
using System.Threading.Tasks;

namespace Hotel.UI
{
    public class BookingCard : MonoBehaviour
    {
        [SerializeField] private TMP_Text _roomName;
        [SerializeField] private TMP_Text _customerEmail;
        [SerializeField] private TMP_Text _customerPhoneNumber;
        [SerializeField] private TMP_Text _fromToText;
        [Space(10)]
        [SerializeField] private BookingStageButton _pendingButton;
        [SerializeField] private BookingStageButton _acceptButton;
        [SerializeField] private BookingStageButton _declineButton;
        private BookingStageButton _selectedButton;
        private BookingStage _selectedStage;
        private int _bookingId;
        private Func<int, BookingStage, Task<bool>> _setStageFunc;

        public void SetValues(int bookingId, string roomName, string customerEmail, string customerPhoneNumber, string fromToText, 
            BookingStage stage, Func<int, BookingStage, Task<bool>> setStageFunc)
        {
            _bookingId = bookingId;
            _roomName.text = roomName;
            _customerEmail.text = customerEmail;
            _customerPhoneNumber.text = customerPhoneNumber;
            _fromToText.text = fromToText;
            _setStageFunc = setStageFunc;
            _SetStageLocal(stage);
        }

        public void SetStagePending()
        {
            _SetStage(BookingStage.Pending);
        }

        public void SetStageAccepted()
        {
            _SetStage(BookingStage.Accepted);
        }

        public void SetStageDeclined()
        {
            _SetStage(BookingStage.Declined);
        }

        private async void _SetStage(BookingStage stage)
        {
            if (_selectedStage != stage)
            {
                if (await _setStageFunc(_bookingId, stage))
                {
                    _SetStageLocal(stage);
                }
            }
        }

        private void _SetStageLocal(BookingStage stage)
        {
            if (_selectedButton) _selectedButton.Deselect();
            switch (stage)
            {
                case BookingStage.Pending:
                    {
                        _selectedButton = _pendingButton;
                        break;
                    }
                case BookingStage.Accepted: 
                    {
                        _selectedButton = _acceptButton;
                        break;
                    }
                case BookingStage.Declined: 
                    {
                        _selectedButton = _declineButton;
                        break;
                    }
            }
            _selectedButton.Select();
            _selectedStage = stage;
        }
    }
}