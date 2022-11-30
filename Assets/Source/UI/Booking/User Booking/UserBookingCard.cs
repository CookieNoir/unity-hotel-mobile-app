using UnityEngine;
using TMPro;

namespace Hotel.UI
{
    public class UserBookingCard : MonoBehaviour
    {
        [SerializeField] private TMP_Text _roomName;
        [SerializeField] private TMP_Text _bedsNumber;
        [SerializeField] private TMP_Text _price;
        [SerializeField] private TMP_Text _fromToDate;
        [SerializeField] private UserBookingStage _bookingStage;

        public void SetValues(string roomName, string bedsNumber, string price, string fromToDate, int stage)
        {
            _roomName.text = roomName;
            _bedsNumber.text = bedsNumber;
            _price.text = price;
            _fromToDate.text = fromToDate;
            _bookingStage.SetValue(stage);
        }
    }
}