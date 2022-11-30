using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Hotel.UI
{
    public class CalendarHandler : MonoBehaviour
    {
        [SerializeField] private Transform _calendarParent;
        [SerializeField] private GameObject _fillerDayPrefab;
        [SerializeField] private GameObject _weekdayDayPrefab;
        [SerializeField] private GameObject _weekendDayPrefab;
        [SerializeField, Min(1)] private int _daysToSelect;
        [SerializeField] private UnityEvent<DateTime> OnDaySelected;
        private List<CalendarDay> _days;
        private CalendarDay _selectedDay;

        private void Awake()
        {
            _days = new List<CalendarDay>();
        }

        public DateTime GetMaxPossibleDay(DateTime currentDay)
        {
            return currentDay.AddDays(_daysToSelect - 1);
        }

        public void Fill(DateTime currentDay, HashSet<DateTime> busyDays)
        {
            _Drop();
            if (currentDay.DayOfWeek != DayOfWeek.Monday)
            {
                int daysBefore = (int)currentDay.DayOfWeek - 1;
                if (daysBefore < 0) daysBefore += 7;
                for (int i = daysBefore; i > 0; --i)
                {
                    _CreateDayButton(currentDay.AddDays(-i), true);
                }
            }
            int currentDayIndex = _days.Count;
            for (int i = 0; i < _daysToSelect; ++i)
            {
                DateTime dateTime = currentDay.AddDays(i);
                bool isBusy = busyDays.Contains(dateTime.Date);
                _CreateDayButton(dateTime, isBusy);
            }
            DateTime nextDay = currentDay.AddDays(_daysToSelect);
            int daysAfter = 8 - (int)nextDay.DayOfWeek;
            if (daysAfter == 8) daysAfter -= 7;
            for (int i = 0; i < daysAfter; ++i)
            {
                _CreateDayButton(currentDay.AddDays(_daysToSelect + i), true);
            }
            OnSelect(currentDayIndex);
        }

        private void _CreateDayButton(DateTime day, bool isFiller)
        {
            GameObject prefab;
            if (isFiller)
            {
                prefab = _fillerDayPrefab;
            }
            else
            {
                if (day.DayOfWeek == DayOfWeek.Sunday || day.DayOfWeek == DayOfWeek.Saturday)
                    prefab = _weekendDayPrefab;
                else prefab = _weekdayDayPrefab;
            }
            GameObject dayObject = Instantiate(prefab, _calendarParent);
            CalendarDay calendarDay = dayObject.GetComponent<CalendarDay>();
            calendarDay.SetValues(_days.Count, day, day.Day.ToString(), OnSelect);
            _days.Add(calendarDay);
        }

        private void _Drop()
        {
            if (_days.Count == 0) return;

            foreach (CalendarDay day in _days)
            {
                Destroy(day.gameObject);
            }
            _days.Clear();
        }

        public void OnSelect(int dayIndex)
        {
            if (_selectedDay) _selectedDay.Deselect();
            _selectedDay = _days[dayIndex];
            OnDaySelected.Invoke(_selectedDay.Select());
        }
    }
}