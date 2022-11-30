using System.Collections.Generic;
using UnityEngine;
using Hotel.Models;

namespace Hotel.UI
{
    public class UserBookingCardHandler : MonoBehaviour
    {
        [SerializeField] private GameObject _userBookingCardPrefab;
        [SerializeField] private Transform _parentObject;

        public void CreateCards(List<UserBookingData> bookings, string roomNameFormat, string bedsNumberFormat, string priceFormat, string fromToFormat)
        {
            foreach (UserBookingData booking in bookings)
            {
                GameObject newCard = Instantiate(_userBookingCardPrefab, _parentObject);
                UserBookingCard bookingCard = newCard.GetComponent<UserBookingCard>();
                bookingCard.SetValues(
                    string.Format(roomNameFormat, booking.RoomId),
                    string.Format(bedsNumberFormat, booking.BedsNumber),
                    string.Format(priceFormat, booking.Price),
                    string.Format(fromToFormat, booking.FromDate.ToString("dd.MM.yyyy"), booking.ToDate.ToString("dd.MM.yyyy")),
                    (int)booking.Stage);
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