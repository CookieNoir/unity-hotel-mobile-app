using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Hotel.Models;

namespace Hotel.UI
{
    public class RoomFilterHandler : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _fieldDropdown;
        [SerializeField] private TMP_Dropdown _functionDropdown;
        [SerializeField] private TMP_InputField _valueInputField;
        [Space(10)]
        [SerializeField] private RoomCardHandler _roomCardHandler;
        private Dictionary<int, IRoomFilter> _filters;

        private void Awake()
        {
            _filters = new Dictionary<int, IRoomFilter>();
        }

        public void ApplyFilter()
        {
            IRoomFilter roomFilter = _SelectFilter();
            if (roomFilter != null)
            {
                try
                {
                    roomFilter.Value = Convert.ToInt32(_valueInputField.text);
                    _roomCardHandler.ApplyFilter(roomFilter.Filter);
                }
                catch
                {
                    return;
                }
            }
            else _roomCardHandler.ApplyFilter(_PlaceholderFilter);
        }

        private IRoomFilter _SelectFilter()
        {
            IRoomFilter selectedFilter = null;
            int filterType = _fieldDropdown.options.Count * ((int)_fieldDropdown.value) + (int)_functionDropdown.value;
            if (_filters.ContainsKey(filterType)) selectedFilter = _filters[filterType];
            else selectedFilter = _AddFilter(filterType);
            return selectedFilter;
        }

        private IRoomFilter _AddFilter(int filterType)
        {
            IRoomFilter newFilter = null;
            switch (filterType)
            {
                case 0:
                    {
                        newFilter = new BedsNumberGEqualFilter();
                        break;
                    }
                case 1:
                    {
                        newFilter = new BedsNumberLEqualFilter();
                        break;
                    }
                case 2:
                    {
                        newFilter = new PriceGEqualFilter();
                        break;
                    }
                case 3:
                    {
                        newFilter = new PriceLEqualFilter();
                        break;
                    }
            }
            if (newFilter != null) _filters.Add(filterType, newFilter);
            return newFilter;
        }

        private bool _PlaceholderFilter(Room room) => true;
    }
}