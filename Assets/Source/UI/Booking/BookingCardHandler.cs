using System;
using System.Collections.Generic;
using UnityEngine;
using Hotel.Models;
using System.Threading.Tasks;

namespace Hotel.UI
{
    public class BookingCardHandler : MonoBehaviour
    {
        [SerializeField] private GameObject _bookingCardPrefab;
        [SerializeField] private Transform _parentObject;

        public void CreateCards(List<BookingData> bookings, string roomNameFormat, string fromToFormat, 
            Func<int, BookingStage, Task<bool>> setStageFunc)
        {
            foreach (BookingData booking in bookings)
            {
                GameObject newCard = Instantiate(_bookingCardPrefab, _parentObject);
                BookingCard bookingCard = newCard.GetComponent<BookingCard>();
                bookingCard.SetValues(booking.BookingId,
                    string.Format(roomNameFormat, booking.RoomId),
                    booking.CustomerEmail,
                    booking.CustomerPhoneNumber,
                    string.Format(fromToFormat, booking.DateFrom.ToString("dd.MM.yyyy"), booking.DateTo.ToString("dd.MM.yyyy")),
                    booking.BookingStage,
                    setStageFunc);
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